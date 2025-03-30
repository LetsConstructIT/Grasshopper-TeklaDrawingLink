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

namespace GTDrawingLink.Components.Dimensions
{
    public class CreateCurvedDimensionSetComponent : CreateDatabaseObjectComponentBaseNew<CreateCurvedDimensionSetCommand>
    {
        private InsertionMode _mode = InsertionMode.Radial;
        private readonly CurvedDimensionSetHandler _sdsHandler = new CurvedDimensionSetHandler();

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.CreateCurvedDimensionSet;

        public CreateCurvedDimensionSetComponent() : base(ComponentInfos.CreateCurvedDimensionSetComponent)
        {
            SetCustomMessage();
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var dimPts, var pts1, var pts2, var pts3, var distances, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
            if (!mainInputIsCorrect)
            {
                HandleMissingInput();
                return null;
            }

            if (!DrawingInteractor.IsInTheActiveDrawing(inputViews.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var views = new ViewCollection<ViewBase>(inputViews);
            var strategy = GetSolverStrategy(true, dimPts, pts1, pts2, pts3, distances, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<CurvedDimensionSetBase>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var dimension = InsertCurvedDimension(views.Get(path),
                                                      dimPts.GetBranch(i),
                                                      pts1.Get(i, inputMode),
                                                      pts2.Get(i, inputMode),
                                                      pts3.Get(i, inputMode),
                                                      distances.Get(i, inputMode),
                                                      attributes.Get(i, inputMode));

                outputObjects.Add(dimension);
                outputTree.Append(new TeklaDatabaseObjectGoo(dimension), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private CurvedDimensionSetBase InsertCurvedDimension(ViewBase view, List<Point3d> points, Point3d p1, Point3d p2, Point3d p3, double distance, string attributesFile)
        {
            var pointList = new PointList();
            foreach (var point in points)
            {
                var teklaPoint = point.ToTekla();
                teklaPoint.Z = 0;
                pointList.Add(teklaPoint);
            }

            if (_mode == InsertionMode.Radial)
                return _sdsHandler.CreateCurvedDimensionSetRadial(view, p1.ToTekla(), p2.ToTekla(), p3.ToTekla(), pointList, distance, new CurvedDimensionSetRadial.CurvedDimensionSetRadialAttributes(attributesFile));
            else
                return _sdsHandler.CreateCurvedDimensionSetOrthogonal(view, p1.ToTekla(), p2.ToTekla(), p3.ToTekla(), pointList, distance, new CurvedDimensionSetOrthogonal.CurvedDimensionSetOrthogonalAttributes(attributesFile));
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.CurvedRadialMode.Name, RadialModeMenuItem_Clicked, true, _mode == InsertionMode.Radial).ToolTipText = ParamInfos.CurvedRadialMode.Description;
            Menu_AppendItem(menu, ParamInfos.CurvedOrthogonalMode.Name, OrthogonalModeMenuItem_Clicked, true, _mode == InsertionMode.Orthogonal).ToolTipText = ParamInfos.CurvedOrthogonalMode.Description;
            Menu_AppendSeparator(menu);
        }

        private void RadialModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = InsertionMode.Radial;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void OrthogonalModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = InsertionMode.Orthogonal;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case InsertionMode.Radial:
                    Message = "Radial reference lines";
                    break;
                case InsertionMode.Orthogonal:
                    Message = "Orthogonal reference lines";
                    break;
                default:
                    Message = "";
                    break;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.CurvedRadialMode.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.CurvedRadialMode.Name, ref serializedInt);
            _mode = (InsertionMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        enum InsertionMode
        {
            Radial,
            Orthogonal
        }
    }

    public class CreateCurvedDimensionSetCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inDimPoints = new InputTreePoint(ParamInfos.DimensionPoints);
        private readonly InputTreePoint _inArcPoint1 = new InputTreePoint(ParamInfos.ArcPoint1);
        private readonly InputTreePoint _inArcPoint2 = new InputTreePoint(ParamInfos.ArcPoint2);
        private readonly InputTreePoint _inArcPoint3 = new InputTreePoint(ParamInfos.ArcPoint3);
        private readonly InputTreeNumber _inDistances = new InputTreeNumber(ParamInfos.RadialDimensionDistance);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.RadialDimensionAttributes, isOptional: true);

        private readonly OutputTreeParam<CurvedDimensionSetBase> _outDimensions = new OutputTreeParam<CurvedDimensionSetBase>(ParamInfos.CurvedDimensionSet, 0);

        internal (List<ViewBase> views, TreeData<Point3d> dimPts, TreeData<Point3d> pts1, TreeData<Point3d> pts2, TreeData<Point3d> pts3, TreeData<double> distances, TreeData<string> attributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                _inDimPoints.AsTreeData(),
                _inArcPoint1.AsTreeData(),
                _inArcPoint2.AsTreeData(),
                _inArcPoint3.AsTreeData(),
                _inDistances.AsTreeData(),
                _inAttributes.IsEmpty() ? _inAttributes.GetDefault("standard") : _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems() && result.Item4.HasItems() && result.Item5.HasItems() && result.Item6.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure dimensions)
        {
            _outDimensions.Value = dimensions;

            return SetOutput(DA);
        }
    }
}
