using GH_IO.Serialization;
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

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class CreateStraightDimensionSetComponentOLD : CreateDatabaseObjectComponentBase
    {
        private InsertionMode _mode = InsertionMode.Always;
        private StraightDimensionSetHandler _sdsHandler = new StraightDimensionSetHandler();
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.CreateStraightDimensionSet;

        public CreateStraightDimensionSetComponentOLD() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.DimensionLineAlwaysMode.Name, AlwaysModeMenuItem_Clicked, true, _mode == InsertionMode.Always).ToolTipText = ParamInfos.DimensionLineAlwaysMode.Description;
            Menu_AppendItem(menu, ParamInfos.DimensionLineMoreThan2PointsMode.Name, ProjectionLimitModeMenuItem_Clicked, true, _mode == InsertionMode.WhenMoreThan2Points).ToolTipText = ParamInfos.DimensionLineMoreThan2PointsMode.Description;
            Menu_AppendSeparator(menu);
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
                    Message = "";
                    break;
                case InsertionMode.WhenMoreThan2Points:
                    Message = "Only when more than 2 projected points";
                    break;
                default:
                    Message = "";
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
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.tree);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.tree);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.tree));

            SetLastParameterAsOptional(pManager, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.StraightDimensionSet, GH_ParamAccess.tree);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var views = GetInputViews(DA);
            if (views == null || views.Count == 0)
                return null;

            if (!DA.GetDataTree(ParamInfos.DimensionPoints.Name, out GH_Structure<GH_Point> dimPointsTree))
                return null;

            if (!DA.GetDataTree(ParamInfos.DimensionLocation.Name, out GH_Structure<GH_Line> locationsTree))
                return null;

            if (!DA.GetDataTree(ParamInfos.StraightDimensionSetAttributes.Name, out GH_Structure<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>> attributesTree))
                return null;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var insertedDimensions = new List<StraightDimensionSet>();
            for (int i = 0; i < dimPointsTree.Paths.Count; i++)
            {
                var pointsPath = dimPointsTree.Paths[i];
                var viewIdx = pointsPath.Indices.First();
                var view = views.ElementAtOrLast(viewIdx);

                var points = dimPointsTree[i].Select(p => p.Value).ToList();
                var location = GetLocation(locationsTree, i);
                var attributes = GetAttributes(attributesTree, i);

                var dimension = InsertDimensionLine(
                    view,
                    points,
                    location,
                    attributes);

                outputTree.Append(new TeklaDatabaseObjectGoo(dimension), pointsPath);
                insertedDimensions.Add(dimension);
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataTree(0, outputTree);

            return insertedDimensions;
        }

        private Rhino.Geometry.Line GetLocation(GH_Structure<GH_Line> tree, int index)
        {
            if (tree.Branches.Count == 1)
            {
                var branch = tree.Branches.First();
                if (index < branch.Count)
                    return branch[index].Value;
                else
                    return branch.Last().Value;
            }
            else if (index < tree.PathCount)
                return tree[index].First().Value;
            else
                return tree.Last().Value;
        }

        private StraightDimensionSet.StraightDimensionSetAttributes GetAttributes(GH_Structure<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>> tree, int index)
        {
            if (tree.Branches.Count == 0)
            {
                return new StraightDimensionSet.StraightDimensionSetAttributes(null, "standard");
            }
            else if (tree.Branches.Count == 1)
            {
                var branch = tree.Branches.First();
                if (index < branch.Count)
                    return branch[index].Value;
                else
                    return branch.Last().Value;
            }
            else if (index < tree.PathCount)
                return tree[index].First().Value;
            else
                return tree.Last().Value;
        }

        private List<ViewBase> GetInputViews(IGH_DataAccess DA)
        {
            var inputs = DA.GetGooListValue<DatabaseObject>(ParamInfos.View);
            if (inputs == null)
                return null;

            var viewBases = new List<ViewBase>();
            foreach (var viewInput in inputs)
            {
                if (viewInput is ViewBase viewBase)
                {
                    viewBases.Add(viewBase);
                }
                else if (viewInput is Drawing drawing)
                {
                    viewBases.Add(drawing.GetSheet());
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unrecognized View input");
                    return null;
                }
            }

            return viewBases;
        }

        private StraightDimensionSet InsertDimensionLine(ViewBase view, List<Point3d> points, Rhino.Geometry.Line location, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
            {
                var teklaPoint = point.ToTekla();
                teklaPoint.Z = 0;
                pointList.Add(teklaPoint);
            }

            location.FromZ = 0;
            location.ToZ = 0;

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
