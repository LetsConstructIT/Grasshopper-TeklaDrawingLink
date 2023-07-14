using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class CreateStraightDimensionSetComponent : CreateDatabaseObjectComponentBase
    {
        private StraightDimensionSetHandler _sdsHandler = new StraightDimensionSetHandler();
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateStraightDimensionSet;

        public CreateStraightDimensionSetComponent() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.list));
            pManager.AddCurveParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.tree);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.tree);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.tree));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.tree));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RemoveInsertedObjects();
            var views = DA.GetGooListValue<DatabaseObject>(ParamInfos.View).Cast<View>().ToList();
            if (views == null)
                return;

            var dimCurveTree = DA.GetGooDataTreeValue<GH_Curve>(ParamInfos.DimensionPoints);
            var dimPolylineTree = CastToPolylines(dimCurveTree);
            if (dimPolylineTree == null)
                return;

            var dimLinesTree = DA.GetGooDataTreeValue<GH_Line>(ParamInfos.DimensionLocation);
            var dimLocationTree = CastToLines(dimLinesTree);
            if (dimLocationTree == null)
                return;

            var dimAttributesTree = DA.GetGooDataTreeValue<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>>(ParamInfos.StraightDimensionSetAttributes);
            var attributesTree = CastToAttributes(dimAttributesTree);

            if (!CheckInputLength(views, dimPolylineTree, dimLocationTree, attributesTree))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input arguments must have equal length");
                return;
            }

            var insertedDimensions = new List<List<StraightDimensionSet>>();
            for (int i = 0; i < views.Count; i++)
            {
                insertedDimensions.Add(InsertDimensionLines(views[i], dimPolylineTree[i], dimLocationTree[i], attributesTree[i]));
            }

            DrawingInteractor.CommitChanges();

            var outputDataTree = new DataTree<TeklaDatabaseObjectGoo>();
            for (int i = 0; i < insertedDimensions.Count; i++)
            {
                GH_Path pth = new GH_Path(0, i);
                outputDataTree.AddRange(insertedDimensions[i].Select(d => new TeklaDatabaseObjectGoo(d)), pth);
            }

            int paramIndex = base.Params.IndexOfOutputParam(ParamInfos.StraightDimensionSet.Name);
            DA.SetDataTree(paramIndex, outputDataTree);

            AddInsertedObjects(insertedDimensions.SelectMany(d => d));
        }

        private List<StraightDimensionSet> InsertDimensionLines(View view, List<Rhino.Geometry.Polyline> polylines, List<Rhino.Geometry.Line> locations, List<StraightDimensionSet.StraightDimensionSetAttributes> dimAttributes)
        {
            var insertedDimensions = new List<StraightDimensionSet>();
            for (int i = 0; i < polylines.Count; i++)
            {
                var dimPolyline = polylines[i];
                var dimLocation = locations.ElementAtOrLast(i);
                var attributes = dimAttributes.ElementAtOrLast(i);

                var pointList = new PointList();
                foreach (var point in dimPolyline)
                    pointList.Add(point.ToTeklaPoint());

                (Vector vector, double distance) locationProperties = CalculateLocation(dimLocation, dimPolyline.First());

                var sds = _sdsHandler.CreateDimensionSet(view, pointList, locationProperties.vector, locationProperties.distance, attributes);
                insertedDimensions.Add(sds);
            }

            return insertedDimensions;
        }

        private List<List<Rhino.Geometry.Polyline>> CastToPolylines(List<List<GH_Curve>> inputTree)
        {
            var outputTree = new List<List<Rhino.Geometry.Polyline>>();

            foreach (var branch in inputTree)
            {
                var outputList = new List<Rhino.Geometry.Polyline>();
                outputTree.Add(outputList);
                foreach (var element in branch)
                {
                    if (element == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Non existing polyline provided");
                        return null;
                    }

                    if (element.Value is LineCurve lineCurve)
                    {
                        var polylineFromLine = new Rhino.Geometry.Polyline
                        {
                            lineCurve.PointAtStart,
                            lineCurve.PointAtEnd
                        };
                        outputList.Add(polylineFromLine);
                        continue;
                    }

                    if (element.Value is PolylineCurve polylineCurve)
                    {
                        outputList.Add(polylineCurve.ToPolyline());
                        continue;
                    }

                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Curve has to be a polyline");
                    return null;
                }
            }

            return outputTree;
        }

        private List<List<Rhino.Geometry.Line>> CastToLines(List<List<GH_Line>> inputTree)
        {
            var outputTree = new List<List<Rhino.Geometry.Line>>();

            foreach (var branch in inputTree)
            {
                var outputList = new List<Rhino.Geometry.Line>();
                outputTree.Add(outputList);
                foreach (var element in branch)
                {
                    if (element == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Non existing line provided");
                        return null;
                    }

                    outputList.Add(element.Value);
                }
            }

            return outputTree;
        }

        private List<List<StraightDimensionSet.StraightDimensionSetAttributes>> CastToAttributes(List<List<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>>> inputTree)
        {
            var outputTree = new List<List<StraightDimensionSet.StraightDimensionSetAttributes>>();

            foreach (var branch in inputTree)
            {
                var outputList = new List<StraightDimensionSet.StraightDimensionSetAttributes>();
                outputTree.Add(outputList);
                foreach (var element in branch)
                {
                    if (element == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Non existing line provided");
                        return null;
                    }

                    outputList.Add(element.Value);
                }
            }

            return outputTree;
        }

        private bool CheckInputLength(List<View> views, List<List<Rhino.Geometry.Polyline>> dimPolylineTree, List<List<Rhino.Geometry.Line>> dimLocationTree, List<List<StraightDimensionSet.StraightDimensionSetAttributes>> attributesTree)
        {
            return Comparer.AllEqual(
                views.Count,
                dimPolylineTree.Count,
                dimLocationTree.Count,
                attributesTree.Count);
        }

        private (Vector vector, double distance) CalculateLocation(Rhino.Geometry.Line dimLineLocation, Point3d dimPoint)
        {
            var line = new Tekla.Structures.Geometry3d.Line(dimLineLocation.From.ToTeklaPoint(), dimLineLocation.To.ToTeklaPoint());
            var teklaPoint = dimPoint.ToTeklaPoint();
            teklaPoint.Z = 0;
            var projected = Projection.PointToLine(teklaPoint, line);

            var upVector = new Vector(projected - teklaPoint).GetNormal();

            var distance = Distance.PointToPoint(projected, teklaPoint);
            if (distance < Math.Pow(10, -3))
            {
                upVector = line.Direction.Cross(new Vector(0, 0, 1));
            }
            return (upVector, distance);
        }
    }
}
