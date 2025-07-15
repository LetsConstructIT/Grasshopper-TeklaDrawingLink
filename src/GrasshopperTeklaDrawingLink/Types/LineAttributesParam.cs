using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class LineAttributesParam : GH_PersistentParam<LineAttributesGoo>
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

        protected override LineAttributesGoo InstantiateT()
        {
            return new LineAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref LineAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<LineAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
