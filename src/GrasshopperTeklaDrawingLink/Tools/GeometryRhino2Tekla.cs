using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using GTDrawingLink.Extensions;
using Tekla.Structures.Geometry3d;
using System.Linq;

namespace GTDrawingLink.Tools
{
    public static class GeometryRhino2Tekla
    {
        internal static IList<Point> GetMergedBoundaryPoints(this IGH_GeometricGoo boundary, bool openLoops)
        {
            var result = new List<Point>();
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
            else if (boundary is GH_Curve)
            {
                var curve = (boundary as GH_Curve).Value;
                if (curve.TryGetPolyline(out var polyline))
                {
                    result.AddRange(polyline.Select(p => p.ToTekla()));
                }
                else if (curve.IsLinear())
                {
                    result.Add(curve.PointAtStart.ToTekla());
                    result.Add(curve.PointAtEnd.ToTekla());
                }
            }

            return result;
        }
    }
}
