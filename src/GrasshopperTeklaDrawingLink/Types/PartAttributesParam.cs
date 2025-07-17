using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PartAttributesParam : GH_PersistentParam<GH_Goo<Part.PartAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PartAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PartAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Part.PartAttributes> InstantiateT()
        {
            return new PartAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Part.PartAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Part.PartAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
