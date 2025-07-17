using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolInfoParam : GH_PersistentParam<GH_Goo<SymbolInfo>>
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

        protected override GH_Goo<SymbolInfo> InstantiateT()
        {
            return new SymbolInfoGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<SymbolInfo> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<SymbolInfo>> values)
            => GH_GetterResult.cancel;
    }
}
