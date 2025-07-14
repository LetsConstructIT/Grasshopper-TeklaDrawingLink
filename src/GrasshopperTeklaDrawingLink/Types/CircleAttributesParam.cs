using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class CircleAttributesParam : GH_PersistentParam<CircleAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public CircleAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public CircleAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override CircleAttributesGoo InstantiateT()
        {
            return new CircleAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref CircleAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<CircleAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
