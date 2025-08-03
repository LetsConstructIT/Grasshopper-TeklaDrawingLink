using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineTypeAttributesParam : GH_PersistentParam<GH_Goo<LineTypeAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public LineTypeAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public LineTypeAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<LineTypeAttributes> InstantiateT()
        {
            return new LineTypeAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<LineTypeAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<LineTypeAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
