using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class StraightDimensionSetAttributesParam : GH_Param<GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public StraightDimensionSetAttributesParam(IGH_InstanceDescription tag)
			: base(tag)
		{
		}

		public StraightDimensionSetAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag, access)
		{
		}

		protected override GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes> InstantiateT()
		{
			return new StraightDimensionSetAttributesGoo();
		}
	}
}
