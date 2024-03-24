using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using System;
using System.Windows.Forms;
using GTDrawingLink.Tools;
using System.Drawing;

namespace GTDrawingLink.Components.Miscs.Loops
{
    public class LinkParameter : GH_Param<GH_String>
    {
        public override Guid ComponentGuid => new Guid(VersionSpecificConstants.LinkParameter);
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.LoopLink;

        public LinkParameter(IGH_InstanceDescription tag, GH_ParamAccess item) : base(tag, item)
        {
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            menu.Items.Clear();
            Menu_AppendObjectName(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendRuntimeMessages(menu);
            Menu_AppendSeparator(menu);
            Menu_AppendWireDisplay(menu);
            Menu_AppendDisconnectWires(menu);
        }
    }
}
