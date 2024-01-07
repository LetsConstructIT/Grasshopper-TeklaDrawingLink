using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolylineAttributesParam : GH_Param<GH_Goo<Polyline.PolylineAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolylineAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolylineAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Polyline.PolylineAttributes> InstantiateT()
        {
            return new PolylineAttributesGoo();
        }
    }
}
