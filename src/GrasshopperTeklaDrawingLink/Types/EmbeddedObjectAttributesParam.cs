using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class EmbeddedObjectAttributesParam : GH_Param<GH_Goo<EmbeddedObjectAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public EmbeddedObjectAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public EmbeddedObjectAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<EmbeddedObjectAttributes> InstantiateT()
        {
            return new EmbeddedObjectAttributesGoo();
        }
    }
}
