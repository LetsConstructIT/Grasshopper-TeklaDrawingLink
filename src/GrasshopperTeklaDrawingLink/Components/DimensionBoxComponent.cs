using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;

namespace GTDrawingLink.Components
{
    public class DimensionBoxComponent : TeklaComponentBaseNew<DimensionBoxCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DimensionBox;
        public DimensionBoxComponent() : base(ComponentInfos.DimensionBoxComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (InputBox, Lines) = _command.GetInputValues();

            var union = Union(InputBox, Lines);

            _command.SetOutputValues(DA, union);
        }

        private Polyline Union(IGH_GeometricGoo initial, List<IGH_GeometricGoo> curves)
        {
            var transformation = Transform.Identity;
            var boundingBox = initial.GetBoundingBox(transformation);

            foreach (var curve in curves)
                boundingBox.Union(curve.GetBoundingBox(transformation));

            var rectangle = new Rectangle3d(Plane.WorldXY, boundingBox.Min, boundingBox.Max);
            return rectangle.ToPolyline();
        }
    }

    public class DimensionBoxCommand : CommandBase
    {
        private readonly InputParam<IGH_GeometricGoo> _inBox = new InputParam<IGH_GeometricGoo>(ParamInfos.DimensionBoxInitialRectangle);
        private readonly InputListParam<IGH_GeometricGoo> _inLines = new InputListParam<IGH_GeometricGoo>(ParamInfos.DimensionLines);

        private readonly OutputParam<Polyline> _unionRectangle = new OutputParam<Polyline>(ParamInfos.UnionRectangle);

        internal (IGH_GeometricGoo InputBox, List<IGH_GeometricGoo> Lines) GetInputValues()
        {
            return (_inBox.Value,
                    _inLines.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Polyline unionRectangle)
        {
            _unionRectangle.Value = unionRectangle;

            return SetOutput(DA);
        }
    }
}
