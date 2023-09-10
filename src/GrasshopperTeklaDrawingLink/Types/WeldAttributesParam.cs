using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class WeldAttributesParam : GH_Param<GH_Goo<Weld.WeldAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public WeldAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public WeldAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Weld.WeldAttributes> InstantiateT()
        {
            return new WeldAttributesGoo();
        }
    }
}
