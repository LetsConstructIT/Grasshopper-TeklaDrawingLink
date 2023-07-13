using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDrawingLink.Extensions
{
    public static class RhinoTeklaMappers
    {
        public static Tekla.Structures.Geometry3d.Point ToTeklaPoint(this Rhino.Geometry.Point3d point)
        {
            return new Tekla.Structures.Geometry3d.Point(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Point3d ToRhinoPoint(this Tekla.Structures.Geometry3d.Point point)
        {
            return new Rhino.Geometry.Point3d(point.X, point.Y, point.Z);
        }

        public static Tekla.Structures.Geometry3d.Vector ToTeklaVector(this Rhino.Geometry.Vector3d point)
        {
            return new Tekla.Structures.Geometry3d.Vector(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Vector3d ToRhinoVector(this Tekla.Structures.Geometry3d.Vector point)
        {
            return new Rhino.Geometry.Vector3d(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Plane ToRhinoPlane(this Tekla.Structures.Geometry3d.CoordinateSystem coordSystem)
        {
            return new Rhino.Geometry.Plane(
                coordSystem.Origin.ToRhinoPoint(),
                coordSystem.AxisX.ToRhinoVector(),
                coordSystem.AxisY.ToRhinoVector());
        }

        public static Rhino.Geometry.BoundingBox ToRhinoBoundingBox(this Tekla.Structures.Geometry3d.AABB aabb)
        {
            return new Rhino.Geometry.BoundingBox(
                aabb.MinPoint.ToRhinoPoint(),
                aabb.MaxPoint.ToRhinoPoint());
        }
    }
}
