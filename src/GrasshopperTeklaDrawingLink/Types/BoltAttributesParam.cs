using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class BoltAttributesParam : GH_PersistentParam<BoltAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public BoltAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public BoltAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override BoltAttributesGoo InstantiateT()
        {
            return new BoltAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref BoltAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<BoltAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
