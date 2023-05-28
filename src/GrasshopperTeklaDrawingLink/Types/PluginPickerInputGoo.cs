using Grasshopper.Kernel.Types;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PluginPickerInputGoo : GH_Goo<PluginPickerInput>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla drawing plugin picker input";

        public override string TypeName => typeof(PluginPickerInput).ToShortString();

        public PluginPickerInputGoo()
        {
        }

        public PluginPickerInputGoo(PluginPickerInput pickerInput)
            : base(pickerInput)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is PluginPickerInput)
            {
                Value = source as PluginPickerInput;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return Value.GetType().ToShortString();
        }
    }
}
