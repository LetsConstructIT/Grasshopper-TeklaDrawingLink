using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class ArrowAttributesParam : GH_PersistentParam<ArrowAttributesGoo> {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ArrowAttributesParam(GH_InstanceDescription tag)
            : base(tag) {
        }

        public ArrowAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag) 
        {
            Access = access;
        }

        protected override ArrowAttributesGoo InstantiateT() {
            return new ArrowAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref ArrowAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<ArrowAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
