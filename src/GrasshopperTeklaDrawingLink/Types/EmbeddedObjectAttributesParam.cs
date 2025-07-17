using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class EmbeddedObjectAttributesParam : GH_PersistentParam<GH_Goo<EmbeddedObjectAttributes>>
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

        protected override GH_Goo<EmbeddedObjectAttributes> InstantiateT()
        {
            return new EmbeddedObjectAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<EmbeddedObjectAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<EmbeddedObjectAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
