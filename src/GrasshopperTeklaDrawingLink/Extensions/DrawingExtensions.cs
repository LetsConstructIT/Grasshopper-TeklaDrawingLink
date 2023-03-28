using GTDrawingLink.Tools;
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
    }
}
