using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineAttributesParam : GH_Param<GH_Goo<Line.LineAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public LineAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public LineAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<Line.LineAttributes> InstantiateT()
        {
            return new LineAttributesGoo();
        }
    }
}
