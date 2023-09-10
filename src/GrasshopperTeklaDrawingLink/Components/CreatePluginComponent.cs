using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreatePluginComponent : CreateDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.Plugin;


        public CreatePluginComponent() : base(ComponentInfos.CreatePluginComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.PluginName, GH_ParamAccess.item);
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
            pManager.AddParameter(new PluginPickerInputParam(ParamInfos.PickerInput, GH_ParamAccess.list));
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
            AddTextParameter(pManager, ParamInfos.PluginAttributes, GH_ParamAccess.list, true);
            AddTeklaDbObjectParameter(pManager, ParamInfos.ObjectsToSelect, GH_ParamAccess.tree, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Plugin, GH_ParamAccess.list);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            string pluginName = null;
            if (!DA.GetData(ParamInfos.PluginName.Name, ref pluginName))
                return null;

            var viewInputs = DA.GetGooListValue<DatabaseObject>(ParamInfos.View);
            if (viewInputs == null)
                return null;

            var viewBases = new List<ViewBase>();
            foreach (var viewInput in viewInputs)
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

            var pickerInputs = DA.GetGooListValue<PluginPickerInput>(ParamInfos.PickerInput);
            if (pickerInputs == null)
                return null;

            var attributeFileNames = new List<string>();
            DA.GetDataList(ParamInfos.Attributes.Name, attributeFileNames);

            var attributes = new List<Attributes>();
            var inputAttributes = new List<string>();
            if (DA.GetDataList(ParamInfos.PluginAttributes.Name, inputAttributes))
            {
                try
                {
                    foreach (var inputAttribute in inputAttributes)
                    {
                        attributes.Add(Tools.Attributes.Parse(inputAttribute));
                    }
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Attributes parsing error: " + ex.Message);
                    return null;
                }
            }

            var objectsToSelect = DA.GetGooDataTree<GH_Goo<DatabaseObject>>(ParamInfos.ObjectsToSelect);

            var pluginsToInsert = new int[]
            {
                viewBases.Count,
                pickerInputs.Count,
                attributeFileNames.Count,
                attributes.Count,
                objectsToSelect.Count
            }.Max();

            var insertedPlugins = new Plugin[pluginsToInsert];
            for (int i = 0; i < pluginsToInsert; i++)
            {
                var plugin = InsertPlugin(
                    pluginName,
                    viewBases.ElementAtOrLast(i),
                    pickerInputs.ElementAtOrLast(i),
                    attributeFileNames.Count > 0 ? attributeFileNames.ElementAtOrLast(i) : null,
                    attributes.Count > 0 ? attributes.ElementAtOrLast(i) : null,
                    objectsToSelect.Count > 0 ? objectsToSelect.ElementAtOrLast(i) : null);

                insertedPlugins[i] = plugin;
            }

            DA.SetDataList(ParamInfos.Plugin.Name, insertedPlugins.Select(p => new TeklaDatabaseObjectGoo(p)));
            return insertedPlugins;
        }

        private Plugin InsertPlugin(string pluginName,
                                    ViewBase viewBase,
                                    PluginPickerInput pickerInput,
                                    string attributeFileNames,
                                    Attributes attributes,
                                    List<GH_Goo<DatabaseObject>> objectsToSelect)
        {
            var plugin = new Plugin(viewBase, pluginName);
            plugin.SetPickerInput(pickerInput);

            if (!string.IsNullOrEmpty(attributeFileNames))
                plugin.LoadStandardValues(attributeFileNames);

            if (attributes.HasItems())
                ApplyAttributes(plugin, attributes);

            if (objectsToSelect.HasItems())
            {
                var drawingObjects = new ArrayList();
                foreach (var drawingObject in objectsToSelect)
                {
                    if (drawingObject.IsValid)
                        drawingObjects.Add(drawingObject.Value);
                }

                DrawingInteractor.Highlight(drawingObjects);
            }

            plugin.Insert();

            return plugin;
        }

        private void ApplyAttributes(Plugin plugin, Attributes attributes)
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
}
