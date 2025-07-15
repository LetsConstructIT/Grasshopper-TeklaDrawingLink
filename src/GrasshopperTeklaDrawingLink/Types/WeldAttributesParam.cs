using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class WeldAttributesParam : GH_PersistentParam<WeldAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public WeldAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public WeldAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override WeldAttributesGoo InstantiateT()
        {
            return new WeldAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref WeldAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<WeldAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
