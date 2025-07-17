using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ArrowAttributesParam : GH_PersistentParam<GH_Goo<ArrowheadAttributes>> {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ArrowAttributesParam(GH_InstanceDescription tag)
            : base(tag) {
        }

        public ArrowAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag) 
        {
            Access = access;
        }

        protected override GH_Goo<ArrowheadAttributes> InstantiateT() {
            return new ArrowAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<ArrowheadAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<ArrowheadAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
