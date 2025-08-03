using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementMeshAttributesParam : GH_PersistentParam<GH_Goo<ReinforcementBase.ReinforcementMeshAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public ReinforcementMeshAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public ReinforcementMeshAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<ReinforcementBase.ReinforcementMeshAttributes> InstantiateT()
        {
            return new ReinforcementMeshAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<ReinforcementBase.ReinforcementMeshAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<ReinforcementBase.ReinforcementMeshAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
