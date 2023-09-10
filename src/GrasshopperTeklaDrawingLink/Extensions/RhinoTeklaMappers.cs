namespace GTDrawingLink.Extensions
{
    public static class RhinoTeklaMappers
    {
        public static Tekla.Structures.Geometry3d.Point ToTekla(this Rhino.Geometry.Point3d point)
        {
            return new Tekla.Structures.Geometry3d.Point(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Point3d ToRhino(this Tekla.Structures.Geometry3d.Point point)
        {
            return new Rhino.Geometry.Point3d(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Line ToRhino(this Tekla.Structures.Geometry3d.LineSegment line)
        {
            return new Rhino.Geometry.Line(line.Point1.ToRhino(), line.Point2.ToRhino());
        }

        public static Tekla.Structures.Geometry3d.Vector ToTekla(this Rhino.Geometry.Vector3d point)
        {
            return new Tekla.Structures.Geometry3d.Vector(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Vector3d ToRhino(this Tekla.Structures.Geometry3d.Vector point)
        {
            return new Rhino.Geometry.Vector3d(point.X, point.Y, point.Z);
        }

        public static Rhino.Geometry.Plane ToRhino(this Tekla.Structures.Geometry3d.CoordinateSystem coordSystem)
        {
            return new Rhino.Geometry.Plane(
                coordSystem.Origin.ToRhino(),
                coordSystem.AxisX.ToRhino(),
                coordSystem.AxisY.ToRhino());
        }

        public static Tekla.Structures.Geometry3d.CoordinateSystem ToTekla(this Rhino.Geometry.Plane plane)
        {
            return new Tekla.Structures.Geometry3d.CoordinateSystem(
                plane.Origin.ToTekla(),
                plane.XAxis.ToTekla(),
                plane.YAxis.ToTekla());
        }

        public static Rhino.Geometry.BoundingBox ToRhino(this Tekla.Structures.Geometry3d.AABB aabb)
        {
            return new Rhino.Geometry.BoundingBox(
                aabb.MinPoint.ToRhino(),
                aabb.MaxPoint.ToRhino());
        }

        public static Tekla.Structures.Geometry3d.AABB ToTekla(this Rhino.Geometry.BoundingBox plane)
        {
            return new Tekla.Structures.Geometry3d.AABB(
                plane.Min.ToTekla(),
                plane.Max.ToTekla());
        }

        public static Tekla.Structures.Geometry3d.Line ToTekla(this Rhino.Geometry.Line line)
        {
            return new Tekla.Structures.Geometry3d.Line(line.From.ToTekla(), line.To.ToTekla());
        }
    }
}
