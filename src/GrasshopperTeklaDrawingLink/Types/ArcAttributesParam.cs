using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class ArcAttributesParam : GH_PersistentParam<ArcAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ArcAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public ArcAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override ArcAttributesGoo InstantiateT()
        {
            return new ArcAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref ArcAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<ArcAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
