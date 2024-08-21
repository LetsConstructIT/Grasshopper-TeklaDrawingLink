using Rhino;
using System.Collections.Generic;

namespace GTDrawingLink.Extensions
{
    public static class RhinoTeklaMappers
    {
        public static double Scale { get; private set; }
        public static double InvertedScale { get; private set; }

        static RhinoTeklaMappers()
        {
            SetScale();
            try
            {
                RhinoDoc.EndOpenDocument += RhinoDoc_EndOpenDocument;
                RhinoApp.AppSettingsChanged += RhinoApp_AppSettingsChanged;
            }
            catch
            {
            }
        }

        private static bool SetScale(RhinoDoc document = null)
        {
            document ??= RhinoDoc.ActiveDoc;

            double newScale;
            if (document == null)
            {
                newScale = (!Tekla.Structures.Datatype.Settings.TryGetValue("distance_unit", out var teklaUnit)) ?
                    1.0 :
                    ((Tekla.Structures.Datatype.Distance.UnitType)teklaUnit switch
                    {
                        Tekla.Structures.Datatype.Distance.UnitType.Millimeter => 1.0,
                        Tekla.Structures.Datatype.Distance.UnitType.Centimeter => 10.0,
                        Tekla.Structures.Datatype.Distance.UnitType.Meter => 1000.0,
                        Tekla.Structures.Datatype.Distance.UnitType.Inch => 25.4,
                        Tekla.Structures.Datatype.Distance.UnitType.Foot => 304.8,
                        _ => 1.0,
                    });
            }
            else
            {
                var rhinoUnit = document.ModelUnitSystem;
                newScale = ((int)rhinoUnit - 2) switch
                {
                    0 => 1.0,
                    1 => 10.0,
                    2 => 1000.0,
                    3 => 1000000.0,
                    5 => 0.0254,
                    6 => 25.4,
                    7 => 304.8,
                    8 => 1609344.0,
                    _ => 1.0,
                };
            }

            if (newScale != Scale)
            {
                Scale = newScale;
                InvertedScale = 1.0 / newScale;
                return true;
            }

            return false;
        }

        private static void RhinoApp_AppSettingsChanged(object sender, System.EventArgs e)
        {
            SetScale(RhinoDoc.ActiveDoc);
        }

        private static void RhinoDoc_EndOpenDocument(object sender, DocumentOpenEventArgs e)
        {
            SetScale(e.Document);
        }

        public static double ToTekla(this double value)
        {
            return value * Scale;
        }

        public static double ToRhino(this double value)
        {
            return value * InvertedScale;
        }

        public static Tekla.Structures.Geometry3d.Point ToTekla(this Rhino.Geometry.Point3d point)
        {
            return new Tekla.Structures.Geometry3d.Point(point.X * Scale, point.Y * Scale, point.Z * Scale);
        }

        public static Rhino.Geometry.Point3d ToRhino(this Tekla.Structures.Geometry3d.Point point)
        {
            return new Rhino.Geometry.Point3d(point.X * InvertedScale, point.Y * InvertedScale, point.Z * InvertedScale);
        }

        public static Rhino.Geometry.Line ToRhino(this Tekla.Structures.Geometry3d.LineSegment line)
        {
            return new Rhino.Geometry.Line(line.Point1.ToRhino(), line.Point2.ToRhino());
        }

        public static Tekla.Structures.Geometry3d.Vector ToTekla(this Rhino.Geometry.Vector3d point)
        {
            return new Tekla.Structures.Geometry3d.Vector(point.X * Scale, point.Y * Scale, point.Z * Scale);
        }

        public static Rhino.Geometry.Vector3d ToRhino(this Tekla.Structures.Geometry3d.Vector point)
        {
            return new Rhino.Geometry.Vector3d(point.X * InvertedScale, point.Y * InvertedScale, point.Z * InvertedScale);
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

        public static Rhino.Geometry.Plane ToRhino(this Tekla.Structures.Model.Plane plane)
        {
            return new Rhino.Geometry.Plane(
                plane.Origin.ToRhino(),
                plane.AxisX.ToRhino(),
                plane.AxisY.ToRhino());
        }

        public static Tekla.Structures.Model.Plane ToTeklaPlane(this Rhino.Geometry.Plane plane)
        {
            return new Tekla.Structures.Model.Plane()
            {
                Origin = plane.Origin.ToTekla(),
                AxisX = plane.XAxis.ToTekla(),
                AxisY = plane.YAxis.ToTekla()
            };
        }

        public static Rhino.Geometry.Arc ToRhino(this Tekla.Structures.Geometry3d.Arc arc)
        {
            return new Rhino.Geometry.Arc(arc.StartPoint.ToRhino(), arc.ArcMiddlePoint.ToRhino(), arc.EndPoint.ToRhino());
        }

        public static Tekla.Structures.Geometry3d.Arc ToTekla(this Rhino.Geometry.Arc arc)
        {
            return new Tekla.Structures.Geometry3d.Arc(arc.StartPoint.ToTekla(), arc.MidPoint.ToTekla(), arc.EndPoint.ToTekla());
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

        public static Rhino.Geometry.PolylineCurve ToRhino(this Tekla.Structures.Drawing.RectangleBoundingBox boundingBox)
        {
            var points = new List<Rhino.Geometry.Point3d>()
            {
                boundingBox.LowerLeft.ToRhino(),
                boundingBox.UpperLeft.ToRhino(),
                boundingBox.UpperRight.ToRhino(),
                boundingBox.LowerRight.ToRhino(),
                boundingBox.LowerLeft.ToRhino(),
            };
            return new Rhino.Geometry.PolylineCurve(points);
        }
    }
}
