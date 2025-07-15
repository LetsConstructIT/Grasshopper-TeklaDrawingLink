using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class EmbeddedObjectAttributesParam : GH_PersistentParam<EmbeddedObjectAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public EmbeddedObjectAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public EmbeddedObjectAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override EmbeddedObjectAttributesGoo InstantiateT()
        {
            return new EmbeddedObjectAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref EmbeddedObjectAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<EmbeddedObjectAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
