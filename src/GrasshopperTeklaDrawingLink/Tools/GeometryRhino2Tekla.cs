using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using GTDrawingLink.Extensions;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Tools
{
    public static class GeometryRhino2Tekla
    {
        internal static IList<Point> GetMergedBoundaryPoints(this IGH_GeometricGoo boundary, bool openLoops)
        {
            IList<Point> result = new List<Point>();
            if (boundary is GH_Point)
            {
                result.Add((boundary as GH_Point).Value.ToTekla());
            }
            else if (boundary is GH_Line)
            {
                var line = (boundary as GH_Line).Value;
                result.Add(line.From.ToTekla());
                result.Add(line.To.ToTekla());
            }
            else if (boundary is GH_Rectangle)
            {
                var rectangle = (boundary as GH_Rectangle).Value;
                for (int i = 0; i < 4; i++)
                    result.Add(rectangle.Corner(i).ToTekla());

                if (!openLoops)
                    result.Add(rectangle.Corner(0).ToTekla());
            }

            return result;
        }
    }
}
