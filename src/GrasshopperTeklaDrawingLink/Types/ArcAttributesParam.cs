using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ArcAttributesParam : GH_Param<GH_Goo<Arc.ArcAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ArcAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public ArcAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Arc.ArcAttributes> InstantiateT()
        {
            return new ArcAttributesGoo();
        }
    }
}
