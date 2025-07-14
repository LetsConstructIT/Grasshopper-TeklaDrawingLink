using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class FontAttributesParam : GH_PersistentParam<FontAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public FontAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public FontAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override FontAttributesGoo InstantiateT()
        {
            return new FontAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref FontAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<FontAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
