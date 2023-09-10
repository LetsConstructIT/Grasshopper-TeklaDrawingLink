using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolAttributesParam : GH_Param<GH_Goo<SymbolAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public SymbolAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public SymbolAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<SymbolAttributes> InstantiateT()
        {
            return new SymbolAttributesGoo();
        }
    }
}
