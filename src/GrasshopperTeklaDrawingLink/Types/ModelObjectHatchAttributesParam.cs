using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
	public class ModelObjectHatchAttributesParam : GH_Param<GH_Goo<ModelObjectHatchAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public ModelObjectHatchAttributesParam(IGH_InstanceDescription tag)
			: base(tag)
		{
		}

		public ModelObjectHatchAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag, access)
		{
		}

		protected override GH_Goo<ModelObjectHatchAttributes> InstantiateT()
		{
			return new ModelObjectHatchAttributesGoo();
		}
	}
}
