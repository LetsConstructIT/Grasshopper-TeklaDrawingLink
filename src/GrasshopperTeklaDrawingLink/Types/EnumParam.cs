using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class EnumParam<TEnum> : Param_Integer where TEnum : struct, IConvertible
	{
		public override Guid ComponentGuid => new Guid("4B388291-D4F8-45B9-A44F-960B0D0C6FC9");

		public EnumParam(IGH_InstanceDescription tag)
		{
			Name = tag.Name;
			NickName = tag.NickName;
			Description = tag.Description;

			foreach (KeyValuePair<int, string> keyValue in EnumHelpers.GetKeyValues<TEnum>())
				AddNamedValue(keyValue.Value, keyValue.Key);
		}

		public EnumParam(IGH_InstanceDescription tag, GH_ParamAccess access)
			: this(tag)
		{
			Access = access;
		}

		public EnumParam(IGH_InstanceDescription tag, GH_ParamAccess access, object @default)
			: this(tag, access)
		{
			SetPersistentData(@default);
		}

		protected override GH_Integer PreferredCast(object data)
		{
			TEnum? val = EnumHelpers.ObjectToEnumValue<TEnum>(data);
			return new GH_Integer(val.HasValue ? Convert.ToInt32(val) : (-1));
		}
	}
}
