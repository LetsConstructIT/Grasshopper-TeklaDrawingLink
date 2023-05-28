using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTDrawingLink.Extensions;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Tools
{
    public static class GeometryRhino2Tekla
    {
        internal static IList<Point> GetMergedBoundaryPoints(IGH_GeometricGoo boundary)
        {
            IList<Point> result = new List<Point>();
            if (boundary is GH_Point)
            {
                result.Add((boundary as GH_Point).Value.ToTeklaPoint());
            }
            else if (boundary is GH_Line)
            {
                var line = (boundary as GH_Line).Value;
                result.Add(line.From.ToTeklaPoint());
                result.Add(line.To.ToTeklaPoint());
            }

            return result;
        }
    }
}
