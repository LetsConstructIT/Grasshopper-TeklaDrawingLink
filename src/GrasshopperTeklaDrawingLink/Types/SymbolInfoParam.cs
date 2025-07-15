using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class SymbolInfoParam : GH_PersistentParam<SymbolInfoGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public SymbolInfoParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public SymbolInfoParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override SymbolInfoGoo InstantiateT()
        {
            return new SymbolInfoGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref SymbolInfoGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<SymbolInfoGoo> values)
            => GH_GetterResult.cancel;
    }
}
