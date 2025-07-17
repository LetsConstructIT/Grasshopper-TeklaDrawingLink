using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ArcAttributesParam : GH_PersistentParam<GH_Goo<Arc.ArcAttributes>>
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

        protected override GH_Goo<Arc.ArcAttributes>  InstantiateT()
        {
            return new ArcAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Arc.ArcAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Arc.ArcAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
