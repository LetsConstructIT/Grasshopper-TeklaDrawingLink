using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateStraightDimensionSetComponent : CreateDatabaseObjectComponentBaseNew<CreateStraightDimensionSetCommand>
    {
        private InsertionMode _mode = InsertionMode.Always;
        private readonly StraightDimensionSetHandler _sdsHandler = new StraightDimensionSetHandler();

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.CreateStraightDimensionSet;

        public CreateStraightDimensionSetComponent() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
            SetCustomMessage();
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var dimPts, var locations, var attributes) = _command.GetInputValues();

            var strategy = GetSolverStrategy(dimPts, locations, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<StraightDimensionSet>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var dimension = InsertDimensionLine(views.Get(path),
                                                    dimPts.GetBranch(i),
                                                    locations.Get(i, inputMode),
                                                    attributes.Get(i, inputMode));

                outputObjects.Add(dimension);
                outputTree.Append(new TeklaDatabaseObjectGoo(dimension), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
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

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.DimensionLineAlwaysMode.Name, AlwaysModeMenuItem_Clicked, true, _mode == InsertionMode.Always).ToolTipText = ParamInfos.DimensionLineAlwaysMode.Description;
            Menu_AppendItem(menu, ParamInfos.DimensionLineMoreThan2PointsMode.Name, ProjectionLimitModeMenuItem_Clicked, true, _mode == InsertionMode.WhenMoreThan2Points).ToolTipText = ParamInfos.DimensionLineMoreThan2PointsMode.Description;
            Menu_AppendSeparator(menu);
        }

        private void AlwaysModeMenuItem_Clicked(object sender, System.EventArgs e)
        {
            _mode = InsertionMode.Always;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ProjectionLimitModeMenuItem_Clicked(object sender, System.EventArgs e)
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

        enum InsertionMode
        {
            Always,
            WhenMoreThan2Points
        }
    }

    public class CreateStraightDimensionSetCommand : CommandBase
    {
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inDimPoints = new InputTreePoint(ParamInfos.DimensionPoints);
        private readonly InputTreeLine _inLocations = new InputTreeLine(ParamInfos.DimensionLocation);
        private readonly InputOptionalTreeParam<StraightDimensionSet.StraightDimensionSetAttributes> _inAttributes = new InputOptionalTreeParam<StraightDimensionSet.StraightDimensionSetAttributes>(ParamInfos.StraightDimensionSetAttributes);

        private readonly OutputTreeParam<StraightDimensionSet> _outDimensions = new OutputTreeParam<StraightDimensionSet>(ParamInfos.StraightDimensionSet, 0);

        internal (ViewCollection<ViewBase> views, TreeData<Point3d> dimPts, TreeData<Rhino.Geometry.Line> locations, TreeData<StraightDimensionSet.StraightDimensionSetAttributes> attributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                _inDimPoints.AsTreeData(),
                _inLocations.AsTreeData(),
                _inAttributes.IsEmpty() ? _inAttributes.GetDefault(new StraightDimensionSet.StraightDimensionSetAttributes(null, "standard")) : _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure dimensions)
        {
            _outDimensions.Value = dimensions;

            return SetOutput(DA);
        }
    }
}
