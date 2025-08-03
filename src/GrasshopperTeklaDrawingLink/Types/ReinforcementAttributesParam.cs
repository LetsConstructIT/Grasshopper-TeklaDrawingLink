using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementAttributesParam : GH_PersistentParam<GH_Goo<ReinforcementBase.ReinforcementSingleAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public ReinforcementAttributesParam(GH_InstanceDescription tag)
			: base(tag)
		{
		}

		public ReinforcementAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag)
		{
			Access = access;
		}

		protected override GH_Goo<ReinforcementBase.ReinforcementSingleAttributes> InstantiateT()
		{
			return new ReinforcementAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<ReinforcementBase.ReinforcementSingleAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<ReinforcementBase.ReinforcementSingleAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
