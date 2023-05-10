using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
	public class LineTypeAttributesParam : GH_Param<GH_Goo<LineTypeAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public LineTypeAttributesParam(IGH_InstanceDescription tag)
			: base(tag)
		{
		}

		public LineTypeAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag, access)
		{
		}

		protected override GH_Goo<LineTypeAttributes> InstantiateT()
		{
			return new LineTypeAttributesGoo();
		}
	}
}
