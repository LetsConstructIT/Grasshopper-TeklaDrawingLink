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
	public class ModelObjectHatchAttributesParam : GH_PersistentParam<GH_Goo<ModelObjectHatchAttributes>>
	{
		public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

		public ModelObjectHatchAttributesParam(GH_InstanceDescription tag)
			: base(tag)
		{
		}

		public ModelObjectHatchAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag)
		{
			Access = access;
		}

		protected override GH_Goo<ModelObjectHatchAttributes> InstantiateT()
		{
			return new ModelObjectHatchAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<ModelObjectHatchAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<ModelObjectHatchAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
