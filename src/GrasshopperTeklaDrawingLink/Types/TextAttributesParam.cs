using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TextAttributesParam : GH_PersistentParam<GH_Goo<Text.TextAttributes>>
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

        protected override GH_Goo<Text.TextAttributes> InstantiateT()
        {
            return new TextAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Text.TextAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Text.TextAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
