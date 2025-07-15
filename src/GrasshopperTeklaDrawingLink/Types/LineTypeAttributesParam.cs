using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class LineTypeAttributesParam : GH_PersistentParam<LineTypeAttributesGoo>
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

        protected override LineTypeAttributesGoo InstantiateT()
        {
            return new LineTypeAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref LineTypeAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<LineTypeAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
