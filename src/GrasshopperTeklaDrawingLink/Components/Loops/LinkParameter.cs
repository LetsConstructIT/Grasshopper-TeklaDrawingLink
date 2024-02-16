using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using System;
using System.Windows.Forms;
using GTDrawingLink.Tools;
using System.Drawing;

namespace GTDrawingLink.Components.Loops
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
            this.Menu_AppendObjectName(menu);
            GH_DocumentObject.Menu_AppendSeparator(menu);
            this.Menu_AppendRuntimeMessages(menu);
            GH_DocumentObject.Menu_AppendSeparator(menu);
            this.Menu_AppendWireDisplay(menu);
            this.Menu_AppendDisconnectWires(menu);
        }
    }
}
