using System.Linq;

namespace GTDrawingLink.Extensions
{
    internal class Comparer
    {
        public static bool AllEqual<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
                return true;

            return values.All(v => v.Equals(values[0]));
        }
    }
}
