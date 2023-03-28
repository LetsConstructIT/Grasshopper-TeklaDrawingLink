using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingViewBaseParam : GH_Param<GH_Goo<ViewBase>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaDrawingViewBaseParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TeklaDrawingViewBaseParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<ViewBase> InstantiateT()
        {
            return new TeklaDrawingViewBaseGoo();
        }
    }
}
