using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Tools;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class ArrowAttributesParam : GH_Param<GH_Goo<ArrowheadAttributes>> {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ArrowAttributesParam(IGH_InstanceDescription tag)
            : base(tag) {
        }

        public ArrowAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access) {
        }

        protected override GH_Goo<ArrowheadAttributes> InstantiateT() {
            return new ArrowAttributesGoo();
        }
    }
}
