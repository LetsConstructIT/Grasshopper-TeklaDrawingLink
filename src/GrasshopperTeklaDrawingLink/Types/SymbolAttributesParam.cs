using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class SymbolAttributesParam : GH_PersistentParam<SymbolAttributesGoo>
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

        protected override SymbolAttributesGoo InstantiateT()
        {
            return new SymbolAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref SymbolAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<SymbolAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
