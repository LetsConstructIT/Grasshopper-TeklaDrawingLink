using System;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Types
{
    public class TeklaViewParam : GH_Param<GH_Goo<TeklaView>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaViewParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TeklaViewParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<TeklaView> InstantiateT()
        {
            return new TeklaViewGoo();
        }
    }
}
