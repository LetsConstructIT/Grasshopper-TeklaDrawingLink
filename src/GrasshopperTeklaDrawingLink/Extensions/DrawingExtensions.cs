using GTDrawingLink.Tools;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Extensions
{
    internal static class DrawingExtensions
    {
        internal static int GetId(this DatabaseObject drawingObject)
        {
            var identifier = (Tekla.Structures.Identifier)drawingObject.GetPropertyValue("Identifier");
            return identifier.ID;
        }

        internal static Tekla.Structures.Geometry3d.Vector GetUpDirection(this StraightDimensionSet straightDimensionSet)
        {
            return (typeof(StraightDimensionSet).GetProperty("UpDirection", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(straightDimensionSet) as Tekla.Structures.Geometry3d.Vector);
        }

        internal static string GetFilter(this View view)
        {
            return typeof(View).GetMethod("SelectFilter", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(view, null) as string;
        }

        internal static bool SetFilter(this View view, string filterContent)
        {
            return (bool)typeof(View).GetMethod("ModifyFilter", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(view, new object[] { filterContent });
        }
    }
}
