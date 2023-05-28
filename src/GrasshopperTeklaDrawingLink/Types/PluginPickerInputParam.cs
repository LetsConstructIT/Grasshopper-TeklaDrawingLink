using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Properties;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PluginPickerInputParam : GH_Param<GH_Goo<PluginPickerInput>>
    {
        public override Guid ComponentGuid => new Guid("47FFE7E4-5DD7-45DB-ADAD-72922CF2C7FA");

        protected override Bitmap Icon => Resources.PickerInput;

		public PluginPickerInputParam(IGH_InstanceDescription tag)
			: base(tag)
		{
		}

		public PluginPickerInputParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: base(tag, access)
		{
		}

		protected override GH_Goo<PluginPickerInput> InstantiateT()
		{
			return new PluginPickerInputGoo();
		}
	}
}
