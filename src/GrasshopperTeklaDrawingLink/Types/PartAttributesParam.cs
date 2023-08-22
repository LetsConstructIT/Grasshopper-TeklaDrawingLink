using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PartAttributesParam : GH_Param<GH_Goo<Part.PartAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PartAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PartAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Part.PartAttributes> InstantiateT()
        {
            return new PartAttributesGoo();
        }
    }
}
