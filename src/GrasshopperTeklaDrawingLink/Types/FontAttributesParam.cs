using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Tools;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class FontAttributesParam : GH_Param<GH_Goo<FontAttributes>> {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public FontAttributesParam(IGH_InstanceDescription tag)
            : base(tag) {
        }

        public FontAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access) {
        }

        protected override GH_Goo<FontAttributes> InstantiateT() {
            return new FontAttributesGoo();
        }
    }
}
