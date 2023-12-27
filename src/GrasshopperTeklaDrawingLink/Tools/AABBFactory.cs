using System.Collections.Generic;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Tools
{
    internal class AABBFactory
    {
        public static AABB FromPoints(IEnumerable<Point> Points)
        {
            AABB aABB = new AABB();
            foreach (Point Point in Points)
                aABB += Point;

            return aABB;
        }
    }
}
