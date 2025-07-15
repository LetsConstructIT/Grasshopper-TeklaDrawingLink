using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class FrameAttributesParam : GH_PersistentParam<FrameAttributesGoo>
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

        protected override FrameAttributesGoo InstantiateT()
        {
            return new FrameAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref FrameAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<FrameAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
