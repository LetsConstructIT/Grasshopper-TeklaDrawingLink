using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingParam : GH_Param<GH_Goo<Drawing>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaDrawingParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TeklaDrawingParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Drawing> InstantiateT()
        {
            return new TeklaDrawingGoo();
        }
    }
}
