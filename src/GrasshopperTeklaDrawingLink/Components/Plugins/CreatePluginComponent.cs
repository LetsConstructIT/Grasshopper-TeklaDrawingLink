using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.DrawingInternal;

namespace GTDrawingLink.Components.Plugins
{
    public class CreatePluginComponent : CreateDatabaseObjectComponentBaseNew<CreatePluginCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.Plugin;

        public CreatePluginComponent() : base(ComponentInfos.CreatePluginComponent)
        {
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (inputViews, names, pickerInput, attributeFileNames, pluginAttributes, objectsToSelect) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(true, names, pickerInput, attributeFileNames, pluginAttributes, objectsToSelect);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Plugin>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var rawPluginAttribute = pluginAttributes.Get(i, inputMode);
                var pluginAttribute = new Tools.Attributes();
                try
                {
                    pluginAttribute = Tools.Attributes.Parse(rawPluginAttribute);
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Attributes parsing error: " + ex.Message);
                    return null;
                }

                var plugin = InsertPlugin(views.Get(path),
                                          names.Get(i, inputMode),
                                          pickerInput.Get(i, inputMode),
                                          attributeFileNames.Get(i, inputMode),
                                          pluginAttribute,
                                          objectsToSelect.GetBranch(i));

                if (plugin.GetIdentifier().ID == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Drawing plugin was not properly inserted");
                    outputTree.Append(new TeklaDatabaseObjectGoo(null), path);
                }
                else
                {
                    outputObjects.Add(plugin);
                    outputTree.Append(new TeklaDatabaseObjectGoo(plugin), path);
                }
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private Plugin InsertPlugin(ViewBase viewBase,
                                    string pluginName,
                                    PluginPickerInput pickerInput,
                                    string attributeFileName,
                                    Tools.Attributes attributes,
                                    List<DrawingObject> objectsToSelect)
        {
            var plugin = new Plugin(viewBase, pluginName);
            plugin.SetPickerInput(pickerInput);

            if (!string.IsNullOrEmpty(attributeFileName))
                plugin.LoadStandardValues(attributeFileName);

            if (attributes.HasItems())
                ApplyAttributes(plugin, attributes);

            if (objectsToSelect.HasItems() && objectsToSelect.All(o => !(o is null)))
                DrawingInteractor.Highlight(objectsToSelect);

            plugin.Insert();

            return plugin;
        }

        private void ApplyAttributes(Plugin plugin, Tools.Attributes attributes)
        {
            if (attributes == null)
                return;

            foreach (KeyValuePair<string, object> attribute in attributes)
            {
                string key = attribute.Key;
                Type type = attribute.Value.GetType();
                if (type == typeof(string))
                    plugin.SetAttribute(key, (string)attribute.Value);
                else if (type == typeof(int))
                    plugin.SetAttribute(key, (int)attribute.Value);
                else if (type == typeof(double))
                    plugin.SetAttribute(key, (double)attribute.Value);
            }
        }
    }

    public class CreatePluginCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeString _inNames = new InputTreeString(ParamInfos.PluginName, isOptional: true);
        private readonly InputOptionalTreeParam<PluginPickerInput> _pickerInput = new InputOptionalTreeParam<PluginPickerInput>(ParamInfos.PickerInput);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.Attributes, isOptional: true);
        private readonly InputTreeString _inPluginAttributes = new InputTreeString(ParamInfos.PluginAttributes, isOptional: true);
        private readonly InputOptionalTreeParam<DrawingObject> _objectsToSelect = new InputOptionalTreeParam<DrawingObject>(ParamInfos.ObjectsToSelect);

        private readonly OutputTreeParam<Plugin> _outDimensions = new OutputTreeParam<Plugin>(ParamInfos.Plugin, 0);

        internal (List<ViewBase> views, TreeData<string> names, TreeData<PluginPickerInput> pickerInput, TreeData<string> attributeFileNames, TreeData<string> pluginAttributes, TreeData<DrawingObject> objectsToSelect) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                _inNames.AsTreeData(),
                _pickerInput.AsTreeData(),
                _inAttributes.IsEmpty() ? _inAttributes.GetDefault(string.Empty) : _inAttributes.AsTreeData(),
                _inPluginAttributes.IsEmpty() ? _inPluginAttributes.GetDefault(string.Empty) : _inPluginAttributes.AsTreeData(),
                _objectsToSelect.IsEmpty() ? _objectsToSelect.GetDefault(null) : _objectsToSelect.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure dimensions)
        {
            _outDimensions.Value = dimensions;

            return SetOutput(DA);
        }
    }
}
