using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class PolylineAttributesParam : GH_PersistentParam<PolylineAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolylineAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolylineAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override PolylineAttributesGoo InstantiateT()
        {
            return new PolylineAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref PolylineAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<PolylineAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
