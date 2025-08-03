using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class WeldAttributesParam : GH_PersistentParam<GH_Goo<Weld.WeldAttributes>>
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

        protected override GH_Goo<Weld.WeldAttributes> InstantiateT()
        {
            return new WeldAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Weld.WeldAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Weld.WeldAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
