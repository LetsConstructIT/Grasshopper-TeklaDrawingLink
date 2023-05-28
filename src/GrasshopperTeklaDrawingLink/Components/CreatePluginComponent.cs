using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreatePluginComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.Plugin;


        public CreatePluginComponent() : base(ComponentInfos.CreatePluginComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.PluginName, GH_ParamAccess.item);
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddParameter(new PluginPickerInputParam(ParamInfos.PickerInput, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.PluginAttributes, GH_ParamAccess.item, true);
            AddOptionalParameter(pManager, new TeklaDatabaseObjectParam(ParamInfos.ObjectsToSelect, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Plugin, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string pluginName = null;
            if (!DA.GetData(ParamInfos.PluginName.Name, ref pluginName))
                return;

            var viewInput = DA.GetGooValue<DatabaseObject>(ParamInfos.View);
            if (viewInput == null)
                return;

            var pickerInput = DA.GetGooValue<PluginPickerInput>(ParamInfos.PickerInput);
            if (pickerInput == null)
                return;

            string attributeFileNames = null;
            DA.GetData(ParamInfos.Attributes.Name, ref attributeFileNames);

            Attributes attributes = null;
            string inputAttributes = null;
            if (DA.GetData(ParamInfos.PluginAttributes.Name, ref inputAttributes))
            {
                try
                {
                    attributes = Tools.Attributes.Parse(inputAttributes);
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Attributes parsing error: " + ex.Message);
                    return;
                }
            }

            var objectsToSelect = DA.GetGooListValue<DatabaseObject>(ParamInfos.ObjectsToSelect);

            ViewBase viewBase = null;
            if (viewInput is ViewBase)
                viewBase = viewInput as ViewBase;
            else if (viewInput is Drawing drawing)
                viewBase = drawing.GetSheet();
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unrecognized View input");
                return;
            }

            var plugin = new Plugin(viewBase, pluginName);
            plugin.SetPickerInput(pickerInput);

            if (!string.IsNullOrEmpty(attributeFileNames))
                plugin.LoadStandardValues(attributeFileNames);

            if (attributes.HasItems())
                ApplyAttributes(plugin, attributes);

            if (objectsToSelect.HasItems())
            {
                var drawingObjects = new ArrayList();
                foreach (DrawingObject drawingObject in objectsToSelect)
                {
                    if (drawingObject != null)
                        drawingObjects.Add(drawingObject);
                }

                DrawingInteractor.Highlight(drawingObjects);
            }

            plugin.Insert();

            DA.SetData(ParamInfos.Plugin.Name, new TeklaDatabaseObjectGoo(plugin));
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
