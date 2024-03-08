using Grasshopper.Kernel;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Datatype;

namespace GTDrawingLink.Components
{
    public class BrepProjectionBorderComponent : TeklaComponentBaseNew<BrepProjectionBorderCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.BrepProjectionBorder;
        public BrepProjectionBorderComponent() : base(ComponentInfos.BrepProjectionBorderComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Brep brep, Plane plane) = _command.GetInputValues();

            var orientedBrep = ApplyOrientation(brep, plane);
            var mesh = GetMesh(orientedBrep);
            var shadow = GetShadowOutlines(mesh);
            _command.SetOutputValues(DA,
                                     SimplifyPolyline(shadow.Item1),
                                     shadow.Item2.Select(SimplifyPolyline).ToList());
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
            var outlines = mesh.GetOutlines(Plane.WorldXY);
            return (outlines.First(), outlines.Skip(1));
        }
    }

    public class BrepProjectionBorderCommand : CommandBase
    {
        private readonly InputParam<Brep> _inBrep = new InputParam<Brep>(ParamInfos.Brep);
        private readonly InputOptionalStructParam<Plane> _inPlane = new InputOptionalStructParam<Plane>(ParamInfos.ProjectionPlane, Plane.WorldXY);

        private readonly OutputParam<Polyline> _outOuterBorder = new OutputParam<Polyline>(ParamInfos.ProjectedBoundary);
        private readonly OutputListParam<Polyline> _outInnerBorder = new OutputListParam<Polyline>(ParamInfos.ProjectedHole);

        internal (Brep Brep, Plane Plane) GetInputValues()
        {
            return (_inBrep.Value,
                    _inPlane.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Polyline outer, List<Polyline> inners)
        {
            _outOuterBorder.Value = outer;
            _outInnerBorder.Value = inners;

            return SetOutput(DA);
        }
    }
}
