using GTDrawingLink.Tools;
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

        public static AABB Transform(this AABB aabb, Matrix transformationMatrix)
        {
            var min = transformationMatrix.Transform(aabb.MinPoint);
            var max = transformationMatrix.Transform(aabb.MaxPoint);

            return AABBFactory.FromPoints(new Point[] { min, max });
        }
    }
}
