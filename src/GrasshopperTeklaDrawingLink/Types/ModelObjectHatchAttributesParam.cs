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
	public class ModelObjectHatchAttributesParam : GH_PersistentParam<ModelObjectHatchAttributesGoo>
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

		protected override ModelObjectHatchAttributesGoo InstantiateT()
		{
			return new ModelObjectHatchAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref ModelObjectHatchAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<ModelObjectHatchAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
