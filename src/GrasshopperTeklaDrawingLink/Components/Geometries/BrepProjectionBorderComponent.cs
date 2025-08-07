using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components.Geometries
{
    public class BrepProjectionBorderComponent : TeklaComponentBaseNew<BrepProjectionBorderCommand>
    {
        private readonly ProjectionLogic _projectionLogic = new ProjectionLogic();
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.BrepProjectionBorder;
        public BrepProjectionBorderComponent() : base(ComponentInfos.BrepProjectionBorderComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Brep brep, Plane plane) = _command.GetInputValues();

            var mesh = GetMesh(brep);

            var (outer, inners, unionRectangle) = _projectionLogic.CalculateProjection(mesh, plane);
            if (outer is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The outline of a mesh projected against a plane has failed.");
                return;
            }

            _command.SetOutputValues(DA, outer, inners, unionRectangle);
        }

        private Mesh GetMesh(Brep brep)
        {
            var meshParameters = MeshingParameters.QualityRenderMesh;
            var meshes = Mesh.CreateFromBrep(brep, meshParameters);

            var mesh = new Mesh();
            foreach (var item in meshes)
                mesh.Append(item);

            if (!mesh.IsClosed && brep.IsSolid)
                mesh.HealNakedEdges(DocumentTolerance() * 100.0);

            return mesh;
        }
    }

    public class BrepProjectionBorderCommand : CommandBase
    {
        private readonly InputParam<Brep> _inBrep = new InputParam<Brep>(ParamInfos.Brep);
        private readonly InputOptionalStructParam<Plane> _inPlane = new InputOptionalStructParam<Plane>(ParamInfos.ProjectionPlane, Plane.WorldXY);

        private readonly OutputParam<Polyline> _outOuterBorder = new OutputParam<Polyline>(ParamInfos.ProjectedBoundary);
        private readonly OutputListParam<Polyline> _outInnerBorder = new OutputListParam<Polyline>(ParamInfos.ProjectedHole);
        private readonly OutputParam<Polyline> _unionRectangle = new OutputParam<Polyline>(ParamInfos.UnionRectangle);

        internal (Brep Brep, Plane Plane) GetInputValues()
        {
            return (_inBrep.Value,
                    _inPlane.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Polyline outer, List<Polyline> inners, Polyline unionRectangle)
        {
            _outOuterBorder.Value = outer;
            _outInnerBorder.Value = inners;
            _unionRectangle.Value = unionRectangle;

            return SetOutput(DA);
        }
    }

    public class ProjectionLogic
    {
        public (Polyline outer, List<Polyline> inners, Polyline unionRectangle) CalculateProjection(Mesh mesh, Plane plane)
        {
            var orientedMesh = ApplyOrientation(mesh, plane);
            var shadow = GetShadowOutlines(orientedMesh);
            if (shadow.Item1 is null)
                return (null, null, null);

            var outer = SimplifyPolyline(shadow.Item1);
            var inners = shadow.Item2.Select(SimplifyPolyline).ToList();

            var unionRectangle = Union(new List<Polyline>() { outer });

            return (outer, inners, unionRectangle);
        }

        private (Polyline, IEnumerable<Polyline>) GetShadowOutlines(Mesh mesh)
        {
            var plane = new Plane(Plane.WorldXY.Origin, new Vector3d(0, 0, -1));
            var outlines = mesh.GetOutlines(plane);
            if (outlines.Length == 0)
                return (null, null);

            return (outlines.First(), outlines.Skip(1));
        }

        private Polyline SimplifyPolyline(Polyline polyline)
        {
            MergeColinearSegments(polyline, RhinoDoc.ActiveDoc.ModelAngleToleranceRadians, true);
            return polyline;
        }

        private int MergeColinearSegments(Polyline polyline, double angleTolerance, bool includeSeam)
        {
            var initialSegmentCount = polyline.SegmentCount;
            if (initialSegmentCount < 2)
                return 0;

            for (int num = initialSegmentCount - 1; num > 0; num--)
            {
                if (AreColinear(num - 1, num, num + 1))
                    polyline.RemoveAt(num);
            }

            if (!includeSeam || !polyline.IsClosed)
                return initialSegmentCount - polyline.SegmentCount;

            while (true)
            {
                var count = polyline.Count;
                if (count <= 1)
                {
                    polyline.Clear();
                    break;
                }

                if (!AreColinear(count - 2, 0, 1))
                    break;

                polyline.RemoveAt(0);
                polyline[count - 2] = polyline[0];
            }

            return initialSegmentCount - polyline.SegmentCount;

            bool AreColinear(int i0, int i1, int i2)
            {
                var point3d = polyline[i0];
                var point3d2 = polyline[i1];
                var point3d3 = polyline[i2];
                if (point3d2.Equals(point3d) || point3d2.Equals(point3d3))
                    return true;

                var a = point3d2 - point3d;
                var b = point3d3 - point3d2;
                if (Vector3d.VectorAngle(a, b) <= angleTolerance)
                    return true;

                return false;
            }
        }

        private Mesh ApplyOrientation(Mesh mesh, Plane plane)
        {
            if (plane == Plane.WorldXY)
                return mesh;

            var transformation = new Orientation(plane, Plane.WorldXY);

            var orientedBrep = mesh.DuplicateMesh();
            orientedBrep.Transform(transformation.ToMatrix());
            return orientedBrep;
        }

        private Polyline Union(List<Polyline> polylines)
        {
            var boundingBox = BoundingBox.Unset;

            foreach (var polyline in polylines)
                boundingBox.Union(polyline.BoundingBox);

            var rectangle = new Rectangle3d(Plane.WorldXY, boundingBox.Min, boundingBox.Max);
            return rectangle.ToPolyline();
        }
    }

}
