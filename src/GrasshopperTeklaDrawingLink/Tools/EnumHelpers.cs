using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Tools
{
    public static class EnumHelpers
    {
        public static TEnum? GetEnum<TEnum>(this IGH_DataAccess DA, GH_InstanceDescription instanceDescription) where TEnum : struct, IConvertible
        {
            object inputFromGh = null;
            DA.GetData(instanceDescription.Name, ref inputFromGh);
            if (inputFromGh == null)
                return null;

            var enumValue = ObjectToEnumValue<TEnum>(inputFromGh).Value;
            if (Enum.IsDefined(typeof(TEnum), enumValue))
                return enumValue;
            else
                return null;
        }

        public static Dictionary<int, string> GetKeyValues<TEnum>() where TEnum : struct, IConvertible
        {
            var dictionary = new Dictionary<int, string>();
            foreach (int value in Enum.GetValues(typeof(TEnum)))
                dictionary.Add(value, Enum.GetName(typeof(TEnum), value));

            return dictionary;
        }

        public static TEnum? ObjectToEnumValue<TEnum>(object input) where TEnum : struct, IConvertible
        {
            if (input == null)
                return null;

            if (input is GH_Goo<object> goo)
                input = goo.Value;
            else if (input is GH_Goo<string> strGoo)
                input = strGoo.Value;
            else if (input is GH_Goo<int> intGoo)
                input = intGoo.Value;
            else if (input is GH_Goo<double> dblGoo && dblGoo.Value % 1 == 0.0)
                input = (int)dblGoo.Value;

            if (input is int || input is TEnum)
                return (TEnum)input;

            if (double.TryParse(input as string, out var result) && result % 1 == 0.0)
                return (TEnum)(object)(int)result;

            if (input is string stringInput &&
                Enum.TryParse(stringInput, true, out TEnum enumResult))
                return enumResult;

            return null;
        }

        public static string EnumToString<TEnum>(string separator = "\n") where TEnum : struct, IConvertible
        {
            return (from TEnum f in Enum.GetValues(typeof(TEnum))
                    select f.ToString() + " = " + Convert.ChangeType(f, f.GetTypeCode())).Aggregate((string a, string b) => a + separator + b);
        }
    }
}
