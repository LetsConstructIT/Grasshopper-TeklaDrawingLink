using System;
using System.Text.RegularExpressions;

namespace GTDrawingLink
{
    internal static class ExtensionMethods
    {
        public static string ToShortString(this Type type)
        {
            string name = type.Name;
            switch (name)
            {
                case "String":
                    return "string";
                case "Int32":
                    return "int";
                case "Double":
                    return "float";
            }

            if (type.Namespace.StartsWith("Tekla.Structures.Geometry3d"))
                name = $"Tekla {name}";

            return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        }
    }
}
