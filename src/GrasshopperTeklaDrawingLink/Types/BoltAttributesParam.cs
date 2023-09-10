using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class BoltAttributesParam : GH_Param<GH_Goo<Bolt.BoltAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public BoltAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public BoltAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Bolt.BoltAttributes> InstantiateT()
        {
            return new BoltAttributesGoo();
        }
    }
}
