using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineAttributesParam : GH_PersistentParam<GH_Goo<Line.LineAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public LineAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public LineAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Line.LineAttributes> InstantiateT()
        {
            return new LineAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Line.LineAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Line.LineAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
