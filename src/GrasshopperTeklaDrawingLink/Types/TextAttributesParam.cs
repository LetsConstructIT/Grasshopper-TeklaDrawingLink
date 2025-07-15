using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class TextAttributesParam : GH_PersistentParam<TextAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TextAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TextAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access) : base(tag)
        {
            Access = access;
        }

        protected override TextAttributesGoo InstantiateT()
        {
            return new TextAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref TextAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<TextAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
