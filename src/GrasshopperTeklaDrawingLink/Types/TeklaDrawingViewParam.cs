using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingViewParam : GH_Param<TeklaDrawingViewGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaDrawingViewParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TeklaDrawingViewParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }
    }
}
