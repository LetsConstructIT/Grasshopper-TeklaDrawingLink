using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Plugins
{
    public class DeconstructPluginComponent : TeklaComponentBaseNew<DeconstructPluginCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.DeconstructPlugin;

        public DeconstructPluginComponent() : base(ComponentInfos.DeconstructPluginComponent) { }
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var plugin = _command.GetInputValues();
            plugin.Select();

            var localAttributes = AttributesIO.GetAll(plugin);
            var asString = localAttributes.Select(a => a.ToString()).ToList();

            _command.SetOutputValues(DA, plugin.Name, asString);
        }
    }

    public class DeconstructPluginCommand : CommandBase
    {
        private readonly InputParam<Plugin> _inPlugin = new InputParam<Plugin>(ParamInfos.Plugin);

        private readonly OutputParam<string> _outName = new OutputParam<string>(ParamInfos.PluginName);
        private readonly OutputListParam<string> _outAttributes = new OutputListParam<string>(ParamInfos.PluginAttributes);

        internal Plugin GetInputValues()
        {
            return _inPlugin.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string name, List<string> attributes)
        {
            _outName.Value = name;
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
