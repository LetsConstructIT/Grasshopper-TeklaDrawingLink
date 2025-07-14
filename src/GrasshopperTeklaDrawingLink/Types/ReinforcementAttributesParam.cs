using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class ReinforcementAttributesParam : GH_PersistentParam<ReinforcementAttributesGoo>
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

		protected override ReinforcementAttributesGoo InstantiateT()
		{
			return new ReinforcementAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref ReinforcementAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<ReinforcementAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
