using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using Rhino;

namespace GTDrawingLink.Components
{
    public class FindVisibleEdgesComponent : TeklaComponentBaseNew<FindVisibleEdgesCommand>
    {
        private const double _tolerance = 0.0001;

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.FindVisibleEdges;

        public FindVisibleEdgesComponent() : base(ComponentInfos.FindVisibleEdgesComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Curve curve, Vector3d direction) = _command.GetInputValues();
            curve = curve.Simplify(CurveSimplifyOptions.All, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, RhinoDoc.ActiveDoc.ModelAngleToleranceRadians);
            if (!CheckInitialAssumptions(curve, out Polyline polyline))
                return;

            var visibleSegments = FindVisibleSegments(curve, direction, polyline);

            var visiblePoints = GetUniquePoints(visibleSegments);
            var extremes = GetExtremePoints(visiblePoints, direction);

            _command.SetOutputValues(DA, visiblePoints, visibleSegments, extremes);
        }

        private List<Line> FindVisibleSegments(Curve curve, Vector3d direction, Polyline polyline)
        {
            var visibleSegments = new List<Line>();
            var perpVector = GetPerpendicularVector(curve.ClosedCurveOrientation());
            foreach (var segment in polyline.GetSegments())
            {
                var cross = Vector3d.CrossProduct(segment.Direction, perpVector);
                var dot = Vector3d.Multiply(cross, direction);
                if (dot < -1 * _tolerance)
                    visibleSegments.Add(segment);
            }

            return visibleSegments;
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

            var line = new Line(visiblePoints.First(), Vector3d.CrossProduct(direction, Vector3d.ZAxis), 1);

            var ptsWithProjection = visiblePoints.ToDictionary(p => p, p => line.ClosestPoint(p, false));
            if (!Line.TryFitLineToPoints(ptsWithProjection.Values, out Line fitLine))
                return new List<Point3d>();

            var tolerance = _tolerance;
            var points = new List<Point3d>();
            foreach (var ptWithProjection in ptsWithProjection)
            {
                if (fitLine.From.EpsilonEquals(ptWithProjection.Value, tolerance) ||
                    fitLine.To.EpsilonEquals(ptWithProjection.Value, tolerance))
                {
                    points.Add(ptWithProjection.Key);
                }
            }

            return points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
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

        private Vector3d GetPerpendicularVector(CurveOrientation curveOrientation)
        {
            return curveOrientation == CurveOrientation.Clockwise ?
                new Vector3d(0, 0, -1) :
                new Vector3d(0, 0, 1);
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
