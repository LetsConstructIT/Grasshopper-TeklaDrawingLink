using System;

namespace GTDrawingLink.Extensions
{
    public static class TypeExtensions
    {
        public static bool InheritsFrom(this Type t, Type baseType)
        {
            if (t.BaseType == null)
            {
                return false;
            }
            else if (t == baseType)
            {
                return true;
            }
            else if (t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition().InheritsFrom(baseType))
            {
                return true;
            }
            else if (t.BaseType.InheritsFrom(baseType))
            {
                return true;
            }
            return false;
        }

        public static bool InheritsFrom<TBaseType>(this Type t)
            => t.InheritsFrom(typeof(TBaseType));
    }
}
