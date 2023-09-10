﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementAttributesParam : GH_Param<GH_Goo<ReinforcementBase.ReinforcementSingleAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public ReinforcementAttributesParam(IGH_InstanceDescription tag)
			: base(tag)
		{
		}

		public ReinforcementAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag, access)
		{
		}

		protected override GH_Goo<ReinforcementBase.ReinforcementSingleAttributes> InstantiateT()
		{
			return new ReinforcementAttributesGoo();
		}
	}
}
