using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace GTDrawingLink.Extensions
{
    public static class TeklaGeometryExtensions
    {
        public static AABB ToAabb(this Solid solid)
        {
            return new AABB(solid.MinimumPoint, solid.MaximumPoint);
        }
    }
}
