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

namespace GTDrawingLink.Components.Annotations
{
    public class CreatePatternLineComponent : CreateDatabaseObjectComponentBaseNew<CreatePatternLineCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.PatternLine;

        public CreatePatternLineComponent() : base(ComponentInfos.CreatePatternLineComponent) { }

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
            var outputObjects = new List<Plugin>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var geometry = geometries.Get(i, inputMode);
                var points = geometry.GetMergedBoundaryPoints(false).ToList();

                var plugin = InsertPlugin(views.Get(path),
                                          points,
                                          attributes.Get(i, inputMode));

                outputObjects.Add(plugin);
                outputTree.Append(new TeklaDatabaseObjectGoo(plugin), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private Plugin InsertPlugin(ViewBase view, List<Tekla.Structures.Geometry3d.Point> points, string attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
                pointList.Add(point);

            var newPluginInput = new PluginPickerInput();
            newPluginInput.Add(new PickerInputNPoints(view, pointList));

            var plugin = new Plugin(view, "Pattern line");
            plugin.SetPickerInput(newPluginInput);

            if (!string.IsNullOrEmpty(attributes))
                plugin.LoadStandardValues(attributes);

            plugin.Insert();
            return plugin;
        }
    }

    public class CreatePatternLineCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Curve, isOptional: true);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.Attributes);

        private readonly OutputTreeParam<DatabaseObject> _outPlugin = new OutputTreeParam<DatabaseObject>(ParamInfos.PatternLine, 0);

        internal (List<ViewBase> View, TreeData<IGH_GeometricGoo> geometries, TreeData<string> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inGeometricGoo.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure plugin)
        {
            _outPlugin.Value = plugin;

            return SetOutput(DA);
        }
    }
}
