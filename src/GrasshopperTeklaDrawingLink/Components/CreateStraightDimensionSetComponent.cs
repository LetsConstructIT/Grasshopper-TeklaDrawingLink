using Grasshopper.Kernel;
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
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.tree);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.list);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.list));

            SetLastParameterAsOptional(pManager, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.list));
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return null;

            var dimGhPointTree = DA.GetGooDataTree<GH_Point>(ParamInfos.DimensionPoints);
            var dimPointsTree = CastToPoints(dimGhPointTree);
            if (dimPointsTree == null)
                return null;

            var dimLocations = DA.GetGooListValue<Rhino.Geometry.Line>(ParamInfos.DimensionLocation);
            if (dimLocations == null)
                return null;

            var attributes = DA.GetGooListValue<StraightDimensionSet.StraightDimensionSetAttributes>(ParamInfos.StraightDimensionSetAttributes);
            if (attributes == null)
            {
                attributes = new List<StraightDimensionSet.StraightDimensionSetAttributes>
                {
                    new StraightDimensionSet.StraightDimensionSetAttributes(modelObject:null, "standard")
                };
            }

            if (dimPointsTree.Count != dimLocations.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input points must have equal length as locations");
                return null;
            }

            var insertedDimensions = new List<StraightDimensionSet>();
            for (int i = 0; i < dimPointsTree.Count; i++)
            {
                insertedDimensions.Add(InsertDimensionLine(view, dimPointsTree[i], dimLocations[i], attributes.ElementAtOrLast(i)));
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.StraightDimensionSet.Name, insertedDimensions);

            return insertedDimensions;
        }

        private StraightDimensionSet InsertDimensionLine(View view, List<Rhino.Geometry.Point3d> points, Rhino.Geometry.Line location, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
                pointList.Add(point.ToTeklaPoint());

            (Vector vector, double distance) = CalculateLocation(location, points.First());

            return _sdsHandler.CreateDimensionSet(view, pointList, vector, distance, attributes);
        }

        private List<List<Point3d>> CastToPoints(List<List<GH_Point>> inputTree)
        {
            var outputTree = new List<List<Point3d>>();

            foreach (var branch in inputTree)
            {
                var outputList = new List<Point3d>();
                outputTree.Add(outputList);
                foreach (var element in branch)
                {
                    if (element == null || !element.IsValid)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Non existing point provided");
                        return null;
                    }

                    outputList.Add(element.Value);
                }
            }

            return outputTree;
        }

        private List<StraightDimensionSet.StraightDimensionSetAttributes> CastToAttributes(List<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>> inputTree)
        {
            var outputList = new List<StraightDimensionSet.StraightDimensionSetAttributes>();
            foreach (var element in inputTree)
            {
                if (element == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Non existing line provided");
                    return null;
                }

                outputList.Add(element.Value);
            }

            return outputList;
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
