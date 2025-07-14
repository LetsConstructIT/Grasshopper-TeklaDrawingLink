using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class PolygonAttributesParam : GH_PersistentParam<PolygonAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolygonAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolygonAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override PolygonAttributesGoo InstantiateT()
        {
            return new PolygonAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref PolygonAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<PolygonAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
