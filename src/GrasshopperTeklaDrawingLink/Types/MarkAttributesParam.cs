using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Tools;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class MarkAttributesParam : GH_Param<GH_Goo<Mark.MarkAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public MarkAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public MarkAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Mark.MarkAttributes> InstantiateT()
        {
            return new MarkAttributesGoo();
        }
    }

    public class MarkBaseAttributesParam : GH_Param<GH_Goo<MarkBase.MarkBaseAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public MarkBaseAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public MarkBaseAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<MarkBase.MarkBaseAttributes> InstantiateT()
        {
            return new MarkBaseAttributesGoo();
        }
    }
}
