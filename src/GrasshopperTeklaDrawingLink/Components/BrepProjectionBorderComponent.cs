using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components
{
    public class BrepProjectionBorderComponent : TeklaComponentBaseNew<BrepProjectionBorderCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.BrepProjectionBorder;
        public BrepProjectionBorderComponent() : base(ComponentInfos.BrepProjectionBorderComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (TreeData<GH_Brep> brepTree, Plane plane) = _command.GetInputValues();

            var outerTree = new GH_Structure<GH_Curve>();
            var innerTree = new GH_Structure<GH_Curve>();
            for (int i = 0; i < brepTree.Objects.Count; i++)
            {
                var branchObjects = brepTree.Objects[i];
                var path = brepTree.Paths[i];

                var outers = new List<Polyline>();
                var holes = new List<List<Polyline>>();
                for (int j = 0; j < branchObjects.Count; j++)
                {
                    var brep = branchObjects[j].Value;
                    var orientedBrep = ApplyOrientation(brep, plane);
                    var mesh = GetMesh(orientedBrep);
                    var shadow = GetShadowOutlines(mesh);

                    var outer = SimplifyPolyline(shadow.Item1);
                    var inners = shadow.Item2.Select(SimplifyPolyline).ToList();

                    outerTree.Append(new GH_Curve(outer), path);
                    var holePath = path.AppendElement(j);
                    innerTree.AppendRange(inners, holePath);
                }
            }

            _command.SetOutputValues(DA, outerTree, innerTree);
        }

        private GH_Curve SimplifyPolyline(Polyline polyline)
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
            return new GH_Curve(new Polyline(cornerPts).ToPolylineCurve());
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
        private readonly InputTreeParam<GH_Brep> _inBrep = new InputTreeParam<GH_Brep>(ParamInfos.Brep);
        private readonly InputOptionalStructParam<Plane> _inPlane = new InputOptionalStructParam<Plane>(ParamInfos.ProjectionPlane, Plane.WorldXY);

        private readonly OutputTreeParam<Polyline> _outOuterBorder = new OutputTreeParam<Polyline>(ParamInfos.ProjectedBoundary, 0);
        private readonly OutputTreeParam<Polyline> _outInnerBorder = new OutputTreeParam<Polyline>(ParamInfos.ProjectedHole, 1);

        internal (TreeData<GH_Brep> BrepTree, Plane Plane) GetInputValues()
        {
            return (_inBrep.AsTreeData(),
                    _inPlane.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure outer, IGH_Structure inners)
        {
            _outOuterBorder.Value = outer;
            _outInnerBorder.Value = inners;

            return SetOutput(DA);
        }
    }
}
