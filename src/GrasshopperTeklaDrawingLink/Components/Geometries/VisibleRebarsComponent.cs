using Grasshopper.Kernel;
using GTDrawingLink.Components.Parts;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Geometries
{
    public class VisibleRebarsComponent : TeklaComponentBaseNew<VisibleRebarsCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.VisibleRebars;

        public VisibleRebarsComponent() : base(ComponentInfos.VisibleRebarsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawingRebar = _command.GetInputValues();

            var fatherView = drawingRebar.GetView();
            if (!(fatherView is View view))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided rebar does not have valid father view");
                return;
            }

            var coord = view.DisplayCoordinateSystem;
            var modelRebar = ConvertDrawingToModelObjectComponent.ConvertToModelRebar(drawingRebar);

        }
    }

    public class VisibleRebarsCommand : CommandBase
    {
        private readonly InputParam<ReinforcementBase> _inReinforcementObject = new InputParam<ReinforcementBase>(ParamInfos.DrawingReinforcementObject);

        private readonly OutputListParam<Rhino.Geometry.Polyline> _outGeometry = new OutputListParam<Rhino.Geometry.Polyline>(ParamInfos.ReinforcementGeometry);
        private readonly OutputListParam<double> _outBendingRadiuses = new OutputListParam<double>(ParamInfos.ReinforcementBendingRadius);
        internal ReinforcementBase GetInputValues()
        {
            return _inReinforcementObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Rhino.Geometry.Polyline> geometries, List<double> bendingRadiuses)
        {
            _outGeometry.Value = geometries;
            _outBendingRadiuses.Value = bendingRadiuses;

            return SetOutput(DA);
        }
    }
}
