using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolInfoParam : GH_Param<GH_Goo<SymbolInfo>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public SymbolInfoParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public SymbolInfoParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<SymbolInfo> InstantiateT()
        {
            return new SymbolInfoGoo();
        }
    }
}
