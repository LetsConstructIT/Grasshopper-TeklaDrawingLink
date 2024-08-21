using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolygonAttributesParam : GH_Param<GH_Goo<Polygon.PolygonAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolygonAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolygonAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Polygon.PolygonAttributes> InstantiateT()
        {
            return new PolygonAttributesGoo();
        }
    }


}
