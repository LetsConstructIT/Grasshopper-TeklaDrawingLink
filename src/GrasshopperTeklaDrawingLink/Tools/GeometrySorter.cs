using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Tools
{
    public class GeometrySorter
    {
        public List<Point3d> OrderGeometries(List<Point3d> points, Vector3d vector)
        {
            return OrderGeometries(points.Select(p => (IGH_GeometricGoo)new GH_Point(p)).ToList(), vector)
                .Select(p => (p as GH_Point).Value).ToList();
        }

        public List<IGH_GeometricGoo> OrderGeometries(List<IGH_GeometricGoo> geometries, Vector3d vector)
        {
            var transform = GetTransformation(vector);

            var projected = new List<InitialGeometryWithProjection>();
            for (int i = 0; i < geometries.Count; i++)
            {
                var transformed = geometries[i].DuplicateGeometry().Transform(transform);
                var line = new Line(transformed.Boundingbox.Min, transformed.Boundingbox.Max);
                projected.Add(new InitialGeometryWithProjection(geometries[i], line));
            }

            var perpPlane = new Plane(projected.First().Projection.From, vector);
            var ordered = projected.OrderBy(p => p.DistanceTo(perpPlane));

            var orderedGeometries = ordered.Select(o => o.Geometry).ToList();
            return orderedGeometries;
        }

        private Transform GetTransformation(Vector3d vector)
        {
            return new Projection(GetPlane(vector)).ToMatrix();
        }

        private Plane GetPlane(Vector3d vector)
        {
            if (vector.IsParallelTo(Vector3d.ZAxis) == 1)
                return new Plane(Point3d.Origin, vector);
            else
                return new Plane(Point3d.Origin, vector, Vector3d.ZAxis);
        }

        private class InitialGeometryWithProjection
        {
            public IGH_GeometricGoo Geometry { get; }
            public Line Projection { get; }

            public InitialGeometryWithProjection(IGH_GeometricGoo geometry, Line projection)
            {
                Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
                Projection = projection;
            }

            public double DistanceTo(Plane plane)
            {
                var distFrom = plane.DistanceTo(Projection.From);
                var distTo = plane.DistanceTo(Projection.To);

                return Math.Min(distTo, distFrom);
            }
        }
    }
}
