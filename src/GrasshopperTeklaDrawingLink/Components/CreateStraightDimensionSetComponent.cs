using GH_IO.Serialization;
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
        private InsertionMode _mode = InsertionMode.Always;
        private StraightDimensionSetHandler _sdsHandler = new StraightDimensionSetHandler();
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateStraightDimensionSet;

        public CreateStraightDimensionSetComponent() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.DimensionLineAlwaysMode.Name, AlwaysModeMenuItem_Clicked, true, _mode == InsertionMode.Always).ToolTipText = ParamInfos.DimensionLineAlwaysMode.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.DimensionLineMoreThan2PointsMode.Name, ProjectionLimitModeMenuItem_Clicked, true, _mode == InsertionMode.WhenMoreThan2Points).ToolTipText = ParamInfos.DimensionLineMoreThan2PointsMode.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        private void AlwaysModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = InsertionMode.Always;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ProjectionLimitModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = InsertionMode.WhenMoreThan2Points;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case InsertionMode.Always:
                    base.Message = "";
                    break;
                case InsertionMode.WhenMoreThan2Points:
                    base.Message = "Only when more than 2 projected points";
                    break;
                default:
                    base.Message = "";
                    break;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.DimensionLineAlwaysMode.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.DimensionLineAlwaysMode.Name, ref serializedInt);
            _mode = (InsertionMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
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

            var dimensionNumber = new int[] { dimPointsTree.Count, dimLocations.Count }.Max();
            var insertedDimensions = new StraightDimensionSet[dimensionNumber];
            for (int i = 0; i < dimensionNumber; i++)
            {
                var dimension = InsertDimensionLine(
                    view,
                    dimPointsTree.ElementAtOrLast(i),
                    dimLocations.ElementAtOrLast(i),
                    attributes.ElementAtOrLast(i));

                insertedDimensions[i] = dimension;
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.StraightDimensionSet.Name, insertedDimensions);

            return insertedDimensions;
        }

        private StraightDimensionSet InsertDimensionLine(View view, List<Point3d> points, Rhino.Geometry.Line location, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
            {
                var teklaPoint = point.ToTekla();
                teklaPoint.Z = 0;
                pointList.Add(teklaPoint);
            }

            var dimLocation = location.ToTekla();

            if (_mode == InsertionMode.WhenMoreThan2Points && GetUniqueDimensionPoints(pointList, dimLocation).Count == 2)
                return null;

            (Vector vector, double distance) = CalculateLocation(dimLocation, pointList[0]);

            return _sdsHandler.CreateDimensionSet(view, pointList, vector, distance, attributes);
        }

        private List<Tekla.Structures.Geometry3d.Point> GetUniqueDimensionPoints(PointList pointList, Tekla.Structures.Geometry3d.Line dimLocation)
        {
            var uniquePoints = new List<Tekla.Structures.Geometry3d.Point>();

            foreach (Tekla.Structures.Geometry3d.Point point in pointList)
            {
                var projectedPoint = Projection.PointToLine(point, dimLocation);
                if (uniquePoints.All(p => !Tekla.Structures.Geometry3d.Point.AreEqual(p, projectedPoint)))
                {
                    uniquePoints.Add(projectedPoint);
                }
            }


            return uniquePoints;
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

        private (Vector vector, double distance) CalculateLocation(Tekla.Structures.Geometry3d.Line line, Tekla.Structures.Geometry3d.Point teklaPoint)
        {
            var projected = Projection.PointToLine(teklaPoint, line);

            var upVector = new Vector(projected - teklaPoint).GetNormal();

            var distance = Distance.PointToPoint(projected, teklaPoint);
            if (distance < Math.Pow(10, -3))
            {
                upVector = line.Direction.Cross(new Vector(0, 0, 1));
            }
            return (upVector, distance);
        }

        enum InsertionMode
        {
            Always,
            WhenMoreThan2Points
        }
    }
}
