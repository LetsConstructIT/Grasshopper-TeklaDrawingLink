using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Tools;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PlacingBaseParam : GH_Param<GH_Goo<PlacingBase>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PlacingBaseParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PlacingBaseParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<PlacingBase> InstantiateT()
        {
            return new PlacingBaseGoo();
        }
    }
}
