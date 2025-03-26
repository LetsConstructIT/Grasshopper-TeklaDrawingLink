using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Annotations
{
    public class CreatePolygonComponent : CreateDatabaseObjectComponentBaseNew<CreatePolygonCommand>
    {
        private InsertionMode _mode = InsertionMode.Polygon;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Polygon;

        public CreatePolygonComponent() : base(ComponentInfos.CreatePolygonComponent)
        {
            SetCustomMessage();
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var geometries, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, geometries, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<ClosedGraphicObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var polyline = InsertClosedObject(views.Get(path),
                                              geometries.Get(i, inputMode).GetMergedBoundaryPoints(false),
                                              attributes.Get(i, inputMode));

                outputObjects.Add(polyline);
                outputTree.Append(new TeklaDatabaseObjectGoo(polyline), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private ClosedGraphicObject InsertClosedObject(ViewBase view,
                                                              IEnumerable<TSG.Point> points,
                                                              ClosedGraphicObject.ClosedGraphicObjectAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
                pointList.Add(point);

            ClosedGraphicObject closedObject = null;
            switch (_mode)
            {
                case InsertionMode.Polygon:
                    closedObject = new Polygon(view, pointList, new Polygon.PolygonAttributes()
                    {
                        Hatch = attributes.Hatch,
                        Line = attributes.Line,
                    });
                    closedObject.Insert();
                    break;
                case InsertionMode.Cloud:
                    closedObject = new Cloud(view, pointList, new Cloud.CloudAttributes()
                    {
                        Hatch = attributes.Hatch,
                        Line = attributes.Line,
                    });
                    closedObject.Insert();
                    break;
                default:
                    break;
            }

            return closedObject;
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.PolygonMode.Name, PolygonMenuItem_Clicked, true, _mode == InsertionMode.Polygon).ToolTipText = ParamInfos.PolygonMode.Description;
            Menu_AppendItem(menu, ParamInfos.CloudMode.Name, CloudMenuItem_Clicked, true, _mode == InsertionMode.Cloud).ToolTipText = ParamInfos.CloudMode.Description;
            Menu_AppendSeparator(menu);
        }

        private void PolygonMenuItem_Clicked(object sender, System.EventArgs e)
        {
            _mode = InsertionMode.Polygon;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void CloudMenuItem_Clicked(object sender, System.EventArgs e)
        {
            _mode = InsertionMode.Cloud;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            Message = _mode switch
            {
                InsertionMode.Polygon => "Polygon",
                InsertionMode.Cloud => "Cloud",
                _ => "",
            };
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
            Polygon,
            Cloud
        }
    }

    public class CreatePolygonCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Curve, isOptional: true);
        private readonly InputTreeParam<Polygon.PolygonAttributes> _inAttributes = new InputTreeParam<Polygon.PolygonAttributes>(ParamInfos.PolygonAttributes);

        private readonly OutputTreeParam<Polygon> _outPolygons = new OutputTreeParam<Polygon>(ParamInfos.Polygon, 0);

        internal (List<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Polygon.PolygonAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inGeometricGoo.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outPolygons.Value = lines;

            return SetOutput(DA);
        }
    }
}
