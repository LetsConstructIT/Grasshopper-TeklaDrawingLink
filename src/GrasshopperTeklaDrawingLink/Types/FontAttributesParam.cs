using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FontAttributesParam : GH_PersistentParam<GH_Goo<FontAttributes>>
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

        protected override GH_Goo<FontAttributes> InstantiateT()
        {
            return new FontAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<FontAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<FontAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
