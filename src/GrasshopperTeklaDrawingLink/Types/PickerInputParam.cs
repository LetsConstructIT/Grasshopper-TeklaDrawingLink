using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Properties;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PickerInputParam : GH_Param<GH_Goo<PickerInput>>
    {
        public override Guid ComponentGuid => new Guid("945CE0A0-4277-4E82-B5CD-EC1C301A27F3");

        protected override Bitmap Icon => Resources.PickerInputType;

        public PickerInputParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PickerInputParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<PickerInput> InstantiateT()
        {
            return new PickerInputGoo();
        }
    }
}
