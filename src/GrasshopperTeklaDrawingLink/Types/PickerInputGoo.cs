using Grasshopper.Kernel.Types;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PickerInputGoo : GH_Goo<PickerInput>
	{
		public override bool IsValid => true;

		public override string TypeDescription => "Tekla drawing picker input";

		public override string TypeName => typeof(PluginPickerInput).ToShortString();

		public PickerInputGoo()
		{
		}

		public PickerInputGoo(PickerInput pickerInput)
			: base(pickerInput)
		{
		}
		public override IGH_Goo Duplicate()
		{
			throw new NotImplementedException();
		}

		public override bool CastFrom(object source)
		{
			if (source is PickerInput)
			{
				Value = source as PickerInput;
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
