using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components.Geometries
{
    public class BrepProjectionBorderComponent : TeklaComponentBaseNew<BrepProjectionBorderCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.BrepProjectionBorder;
        public BrepProjectionBorderComponent() : base(ComponentInfos.BrepProjectionBorderComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Brep brep, Plane plane) = _command.GetInputValues();

            var orientedBrep = ApplyOrientation(brep, plane);
            var mesh = GetMesh(orientedBrep);
            var shadow = GetShadowOutlines(mesh);

            var outer = SimplifyPolyline(shadow.Item1);
            var inners = shadow.Item2.Select(SimplifyPolyline).ToList();

            var unionRectangle = Union(new List<Polyline>() { outer });

            _command.SetOutputValues(DA, outer, inners, unionRectangle);
        }

        private Polyline SimplifyPolyline(Polyline polyline)
        {
            var angleTolerance = 0;

            var segmentCount = polyline.SegmentCount;
            var cornerPts = new List<Point3d>();

            int num2 = 0;
            Line val3;
            for (int i = 0; i < segmentCount - 2; i++)
            {
                val3 = polyline.SegmentAt(i);
                Vector3d direction = val3.Direction;
                val3 = polyline.SegmentAt(i + 1);
                if (Vector3d.VectorAngle(direction, val3.Direction) > angleTolerance)
                {
                    cornerPts.Add(polyline[i + 1]);
                    num2 = i + 1;
                    break;
                }
            }
            int num3 = 0;
            for (int j = 0; j < segmentCount - 1; j++)
            {
                int num4 = (num2 + j) % segmentCount;
                int num5 = (num4 + 1) % segmentCount;
                val3 = polyline.SegmentAt(num4);
                Vector3d direction2 = val3.Direction;
                val3 = polyline.SegmentAt(num5);
                if (Vector3d.VectorAngle(direction2, val3.Direction) > angleTolerance)
                {
                    cornerPts.Add(polyline[num4 + 1]);
                    num3++;
                }
            }

            cornerPts.Add(cornerPts.First());
            return new Polyline(cornerPts);
        }

        private Brep ApplyOrientation(Brep brep, Plane plane)
        {
            if (plane == Plane.WorldXY)
                return brep;

            var transformation = new Orientation(plane, Plane.WorldXY);

            var orientedBrep = brep.DuplicateBrep();
            orientedBrep.Transform(transformation.ToMatrix());
            return orientedBrep;
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

        private (Polyline, IEnumerable<Polyline>) GetShadowOutlines(Mesh mesh)
        {
            var plane = new Plane(Plane.WorldXY.Origin, new Vector3d(0, 0, -1));
            var outlines = mesh.GetOutlines(plane);
            return (outlines.First(), outlines.Skip(1));
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
}
