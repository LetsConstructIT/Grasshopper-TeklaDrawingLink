using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;

namespace GTDrawingLink.Components.Geometries
{
    public class MeshProjectionBorderComponent : TeklaComponentBaseNew<MeshProjectionBorderCommand>
    {
        private readonly ProjectionLogic _projectionLogic = new ProjectionLogic();
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.MeshProjectionBorder;
        public MeshProjectionBorderComponent() : base(ComponentInfos.MeshProjectionBorderCommand) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Mesh mesh, Plane plane) = _command.GetInputValues();

            var (outer, inners, unionRectangle) = _projectionLogic.CalculateProjection(mesh, plane);
            if (outer is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The outline of a mesh projected against a plane has failed.");
                return;
            }

            _command.SetOutputValues(DA, outer, inners, unionRectangle);
        }
    }

    public class MeshProjectionBorderCommand : CommandBase
    {
        private readonly InputParam<Mesh> _inMesh = new InputParam<Mesh>(ParamInfos.RhinoMesh);
        private readonly InputOptionalStructParam<Plane> _inPlane = new InputOptionalStructParam<Plane>(ParamInfos.ProjectionPlane, Plane.WorldXY);

        private readonly OutputParam<Polyline> _outOuterBorder = new OutputParam<Polyline>(ParamInfos.ProjectedBoundary);
        private readonly OutputListParam<Polyline> _outInnerBorder = new OutputListParam<Polyline>(ParamInfos.ProjectedHole);
        private readonly OutputParam<Polyline> _unionRectangle = new OutputParam<Polyline>(ParamInfos.UnionRectangle);

        internal (Mesh Mesh, Plane Plane) GetInputValues()
        {
            return (_inMesh.Value,
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
