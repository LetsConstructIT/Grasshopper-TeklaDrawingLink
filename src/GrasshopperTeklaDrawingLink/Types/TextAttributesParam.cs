using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Tools;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class TextAttributesParam : GH_Param<GH_Goo<Text.TextAttributes>> {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TextAttributesParam(IGH_InstanceDescription tag)
            : base(tag) {
        }

        public TextAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access) {
        }

        protected override GH_Goo<Text.TextAttributes> InstantiateT() {
            return new TextAttributesGoo();
        }
    }
}
