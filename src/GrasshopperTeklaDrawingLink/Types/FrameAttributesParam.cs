using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FrameAttributesParam : GH_PersistentParam<GH_Goo<Frame>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public FrameAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public FrameAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Frame> InstantiateT()
        {
            return new FrameAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Frame> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Frame>> values)
            => GH_GetterResult.cancel;
    }
}
