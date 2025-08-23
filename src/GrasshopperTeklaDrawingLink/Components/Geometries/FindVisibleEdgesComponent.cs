using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Rhino;
using Rhino.Geometry.Intersect;
using System;

namespace GTDrawingLink.Components.Geometries
{
    public class FindVisibleEdgesComponent : TeklaComponentBaseNew<FindVisibleEdgesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.FindVisibleEdges;

        public FindVisibleEdgesComponent() : base(ComponentInfos.FindVisibleEdgesComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Curve inputCurve, Vector3d direction) = _command.GetInputValues();

            var curve = SimplifyCurve(inputCurve.DuplicateCurve());
            if (!CheckInitialAssumptions(curve, out Polyline polyline))
                return;
            if (curve.ClosedCurveOrientation() == CurveOrientation.CounterClockwise)
                polyline.Reverse();

            var localPlane = CalculatePlane(direction, inputCurve);

            var transformation = Transform.ChangeBasis(Plane.WorldXY, localPlane);
            var reverseTransformation = Transform.ChangeBasis(localPlane, Plane.WorldXY);

            polyline.Transform(transformation);

            var allSegments = polyline.GetSegments();
            var visibleSegments = FindVisibleSegmentsByCrossProduct(allSegments, Vector3d.YAxis);
            visibleSegments = LimitByHiddenSegments(visibleSegments, allSegments, polyline.BoundingBox, Vector3d.YAxis);

            var outputSegments = new List<Line>();
            foreach (var segment in visibleSegments)
            {
                segment.Transform(reverseTransformation);
                outputSegments.Add(segment);
            }

            var visiblePoints = GetUniquePoints(outputSegments);
            var extremes = GetExtremePoints(visiblePoints, direction);

            _command.SetOutputValues(DA, visiblePoints, outputSegments, extremes);
        }

        private Plane CalculatePlane(Vector3d direction, Curve inputCurve)
        {
            var localX = Vector3d.CrossProduct(-Vector3d.ZAxis, direction);

            var plane = new Plane(Point3d.Origin, localX, direction);
            var reverseTransformation = Transform.ChangeBasis(plane, Plane.WorldXY);

            var localBoundingBox = inputCurve.GetBoundingBox(Transform.ChangeBasis(Plane.WorldXY, plane));

            var origin = localBoundingBox.Min;
            origin.Transform(reverseTransformation);

            plane.Origin = origin;
            return plane;
        }

        private Curve SimplifyCurve(Curve curve)
        {
            var simplified = curve.Simplify(CurveSimplifyOptions.All, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, RhinoDoc.ActiveDoc.ModelAngleToleranceRadians);
            if (simplified != null)
                return simplified;
            else
                return curve;
        }

        private List<Line> FindVisibleSegmentsByCrossProduct(Line[] allSegments, Vector3d direction)
        {
            var visibleSegments = new List<Line>();
            var perpVector = new Vector3d(0, 0, -1);
            foreach (var segment in allSegments)
            {
                var segDir = segment.Direction;
                segDir.Unitize();
                var cross = Vector3d.CrossProduct(segDir, perpVector);
                cross.Unitize();
                var dot = Vector3d.Multiply(cross, direction);
                if (dot < -1 * RhinoDoc.ActiveDoc.ModelRelativeTolerance)
                    visibleSegments.Add(segment);
            }

            return visibleSegments;
        }

        private List<Line> LimitByHiddenSegments(List<Line> possibleSegments, Line[] allSegments, BoundingBox boundingBox, Vector3d direction)
        {
            var possibleLines = TransformToLineWithRanges(possibleSegments);

            for (int i = 0; i < possibleLines.Count; i++)
            {
                var line = possibleLines[i];

                for (int j = 0; j < possibleLines.Count; j++)
                {
                    if (i == j) continue;

                    var cover = possibleLines[j];

                    if (!line.RangeX.CollidesWith(cover.RangeX))
                        continue;

                    var isCoveredAtLineFrom = IsCoveredByLine(line.Line, cover.Line, CreateRayThrough(line.Line.From));
                    var isCoveredAtLineTo = IsCoveredByLine(line.Line, cover.Line, CreateRayThrough(line.Line.To));
                    var isCoveredAtCoverFrom = IsCoveredByLine(line.Line, cover.Line, CreateRayThrough(cover.Line.To));
                    var isCoveredAtCoverTo = IsCoveredByLine(line.Line, cover.Line, CreateRayThrough(cover.Line.To));

                    if (isCoveredAtLineFrom && isCoveredAtLineTo) // completly covered
                        line.AddCoveredRangeX(new DoubleRange(line.Line.FromX, line.Line.ToX));
                    else if(isCoveredAtLineTo)
                        line.AddCoveredRangeX(new DoubleRange(cover.Line.FromX, line.Line.ToX));
                    else if (isCoveredAtLineFrom)
                        line.AddCoveredRangeX(new DoubleRange(cover.Line.ToX, line.Line.FromX));
                    else if (isCoveredAtCoverFrom && isCoveredAtCoverTo)
                        line.AddCoveredRangeX(new DoubleRange(cover.Line.ToX, cover.Line.FromX));
                }
            }

            var result = new List<Line>();
            foreach (var possibleLine in possibleLines)
            {
                var line = possibleLine.Line;
                if (possibleLine.CoveredRangesX.Count == 0)
                {
                    result.Add(line);
                    continue;
                }

                var newRanges = possibleLine.RangeX.SubtractMany(possibleLine.CoveredRangesX).ToList();
                foreach (var newRange in newRanges)
                {
                    DoesHaveIntersection(line, CreateRayThrough(new Point3d(newRange.Min, 0, 0)), out Point3d intersection1);
                    DoesHaveIntersection(line, CreateRayThrough(new Point3d(newRange.Max, 0, 0)), out Point3d intersection2);

                    result.Add(new Line(intersection1, intersection2));
                }
            }

            return result;

            Line CreateRayThrough(Point3d vertex)
            {
                var ray = new Line(vertex, direction, 1000);
                ray.ExtendThroughBox(boundingBox, 1000);
                return ray;
            }
        }

        private List<LineWithRanges> TransformToLineWithRanges(List<Line> segments)
        {
            return segments
                 .Select(s => new LineWithRanges(s)).OrderBy(s => s.RangeY.Min)
                 .Where(s => !s.RangeX.IsZero())
                 .ToList();
        }

        private bool IsCoveredByLine(Line lineToCheck, Line coverLine, Line ray)
        {
            var intersectAtCheck = DoesHaveIntersection(lineToCheck, ray, out Point3d intersectionAtCheck);
            var intersectAtCover = DoesHaveIntersection(coverLine, ray, out Point3d intersectionAtCover);

            return intersectAtCheck && intersectAtCover && intersectionAtCover.Y < intersectionAtCheck.Y;
        }

        private bool DoesHaveIntersection(Line lineA, Line lineB, out Point3d intersection)
        {
            intersection = default;
            if (Intersection.LineLine(lineA, lineB, out double a, out double b, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, true))
            {
                intersection = lineA.PointAt(a);
                return true;
            }

            return false;
        }

        private List<Point3d> GetUniquePoints(IEnumerable<Line> segments)
        {
            var points = new HashSet<Point3d>();
            foreach (var segment in segments)
            {
                points.Add(segment.From);
                points.Add(segment.To);
            }

            return points.ToList();
        }

        private List<Point3d> GetExtremePoints(List<Point3d> visiblePoints, Vector3d direction)
        {
            if (!visiblePoints.Any())
                return new List<Point3d>();

            var sortDir = Vector3d.CrossProduct(direction, Vector3d.ZAxis);
            var points = new GeometrySorter().OrderGeometries(visiblePoints, sortDir);

            var extremes = new List<Point3d>()
            {
                points.First(),
                points.Last()
            };

            return extremes.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
        }

        private bool CheckInitialAssumptions(Curve curve, out Polyline? polyline)
        {
            polyline = null;
            if (!curve.IsClosed)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only closed curves can be analyzed");
                return false;
            }

            if (!curve.IsPolyline() || !curve.TryGetPolyline(out polyline))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only polyline curves can be analyzed");
                return false;
            }

            return true;
        }

        private class LineWithRanges
        {
            public Line Line { get; }
            public DoubleRange RangeX { get; }
            public DoubleRange RangeY { get; }
            public List<DoubleRange> CoveredRangesX { get; }

            public LineWithRanges(Line line)
            {
                Line = line;
                RangeX = new DoubleRange(line.FromX, line.ToX);
                RangeY = new DoubleRange(line.FromY, line.ToY);
                CoveredRangesX = new List<DoubleRange>();
            }

            public void AddCoveredRangeX(DoubleRange doubleRange)
            {
                CoveredRangesX.Add(doubleRange);
            }

        }

        private struct DoubleRange
        {
            public double Min { get; }
            public double Max { get; }

            public DoubleRange(double value1, double value2)
            {
                Min = Math.Min(value1, value2);
                Max = Math.Max(value1, value2);
            }

            public double Length() => Max - Min;
            public bool IsZero() => Length() < RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            public bool Contains(DoubleRange other)
            {
                return Min <= other.Min && other.Max <= Max;
            }

            public bool Contains(Double other)
            {
                return Min <= other && other <= Max;
            }

            /// <summary>
            /// Collision occurs only if ranges overlap, not just touch
            /// </summary>
            /// <param name="otherRange"></param>
            /// <returns></returns>
            public bool CollidesWith(DoubleRange otherRange)
            {

                return !(otherRange.Max <= Min || otherRange.Min >= Max);
            }

            public IEnumerable<DoubleRange> Subtract(DoubleRange other)
            {
                // Case 1: no overlap → return this range as is
                if (!CollidesWith(other))
                {
                    yield return this;
                    yield break;
                }

                // Case 2: other fully covers this range → nothing remains
                if (other.Min <= Min && other.Max >= Max)
                {
                    yield break;
                }

                // Case 3: left part remains
                if (other.Min > Min)
                {
                    yield return new DoubleRange(Min, Math.Min(other.Min, Max));
                }

                // Case 4: right part remains
                if (other.Max < Max)
                {
                    yield return new DoubleRange(Math.Max(other.Max, Min), Max);
                }
            }

            public IEnumerable<DoubleRange> SubtractMany(IEnumerable<DoubleRange> toSubtract)
            {
                // Start with just the initial range
                var result = new List<DoubleRange> { this };

                foreach (var other in toSubtract)
                {
                    var next = new List<DoubleRange>();

                    foreach (var range in result)
                    {
                        // Subtract 'other' from each current range
                        next.AddRange(range.Subtract(other));
                    }

                    result = next; // update current results
                }

                return result;
            }
        }
    }

    public class FindVisibleEdgesCommand : CommandBase
    {
        private readonly InputParam<Curve> _inCurve = new InputParam<Curve>(ParamInfos.Curve);
        private readonly InputStructParam<Vector3d> _inVector = new InputStructParam<Vector3d>(ParamInfos.ViewingVector);

        private readonly OutputListParam<Point3d> _outVisiblePoints = new OutputListParam<Point3d>(ParamInfos.VisiblePoints);
        private readonly OutputListParam<Line> _outEdges = new OutputListParam<Line>(ParamInfos.VisibleEdges);
        private readonly OutputListParam<Point3d> _outExtremes = new OutputListParam<Point3d>(ParamInfos.ExtremePoints);

        internal (Curve Curve, Vector3d vector) GetInputValues()
        {
            return (_inCurve.Value,
                    _inVector.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Point3d> visiblePoints, List<Line> visibleEdges, List<Point3d> extremes)
        {
            _outVisiblePoints.Value = visiblePoints;
            _outEdges.Value = visibleEdges;
            _outExtremes.Value = extremes;

            return SetOutput(DA);
        }
    }
}
