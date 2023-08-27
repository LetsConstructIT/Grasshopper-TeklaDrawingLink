using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using TSG = Tekla.Structures.Geometry3d;

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

        internal static IEnumerable<Point> GetPoints(this StraightDimensionSet straightDimensionSet)
        {
            var dimensionPoints = (typeof(StraightDimensionSet).GetProperty("DimensionPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(straightDimensionSet) as PointList).ToArray();

            return dimensionPoints;
        }

        internal static LineSegment GetDimensionLocation(this StraightDimensionSet sds, IEnumerable<Point> dimPoints)
        {
            var upDirection = sds.GetUpDirection();
            var dimLineDirection = upDirection.Cross(new Vector(0, 0, 1));

            var initialPoint = dimPoints.First() + sds.Distance * upDirection;
            var dimLineCoordSystem = new CoordinateSystem(initialPoint, dimLineDirection, upDirection);

            var toDimLocationCs = MatrixFactory.ToCoordinateSystem(dimLineCoordSystem);

            var dimLine = new TSG.Line(initialPoint, dimLineDirection);
            var projectedPoints = dimPoints.Select(p => Projection.PointToLine(p, dimLine)).ToList();
            var localPoints = projectedPoints.Select(p => toDimLocationCs.Transform(p)).ToList();
            var orderedPoints = localPoints.OrderBy(p => p.X);

            var firstPt = projectedPoints[localPoints.IndexOf(orderedPoints.First())];
            var lastPt = projectedPoints[localPoints.IndexOf(orderedPoints.Last())];

            firstPt.Z = 0;
            lastPt.Z = 0;
            return new LineSegment(firstPt, lastPt);
        }

        internal static LineSegment GetDimensionLocation(this StraightDimensionSet sds)
        {
            var dimPoints = sds.GetPoints();
            return sds.GetDimensionLocation(dimPoints);
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
