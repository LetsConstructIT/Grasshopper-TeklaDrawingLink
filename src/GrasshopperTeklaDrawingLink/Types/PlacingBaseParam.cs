using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class PlacingBaseParam : GH_PersistentParam<PlacingBaseGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PlacingBaseParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PlacingBaseParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override PlacingBaseGoo InstantiateT()
        {
            return new PlacingBaseGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref PlacingBaseGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<PlacingBaseGoo> values)
            => GH_GetterResult.cancel;
    }
}
