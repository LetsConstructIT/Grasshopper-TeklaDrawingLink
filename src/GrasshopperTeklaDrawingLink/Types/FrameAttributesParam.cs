using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FrameAttributesParam : GH_Param<GH_Goo<Frame>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public FrameAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public FrameAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Frame> InstantiateT()
        {
            return new FrameAttributesGoo();
        }
    }
}
