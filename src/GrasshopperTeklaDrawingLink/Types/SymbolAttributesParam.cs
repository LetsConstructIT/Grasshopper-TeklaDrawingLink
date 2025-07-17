using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolAttributesParam : GH_PersistentParam<GH_Goo<SymbolAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public SymbolAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public SymbolAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<SymbolAttributes> InstantiateT()
        {
            return new SymbolAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<SymbolAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<SymbolAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
