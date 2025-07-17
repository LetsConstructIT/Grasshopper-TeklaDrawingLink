using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PlacingBaseParam : GH_PersistentParam<GH_Goo<PlacingBase>>
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

        protected override GH_Goo<PlacingBase> InstantiateT()
        {
            return new PlacingBaseGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<PlacingBase> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<PlacingBase>> values)
            => GH_GetterResult.cancel;
    }
}
