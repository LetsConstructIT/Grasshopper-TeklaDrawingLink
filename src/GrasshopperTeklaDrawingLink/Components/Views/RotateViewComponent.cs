using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Views
{
    public class RotateViewComponent : TeklaComponentBaseNew<RotateViewCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.RotateView;

        public RotateViewComponent() : base(ComponentInfos.RotateViewComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (View view, double angle) = _command.GetInputValues();
            view.Select();

            if (!DrawingInteractor.IsInTheActiveDrawing(view))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return;
            }

            view.RotateViewOnDrawingPlane(angle);

            _command.SetOutputValues(DA, view);
        }
    }

    public class RotateViewCommand : CommandBase
    {
        private readonly InputParam<View> _inDrawing = new InputParam<View>(ParamInfos.View);
        private readonly InputStructParam<double> _inAngle = new InputStructParam<double>(ParamInfos.RotationAngle);

        private readonly OutputParam<View> _outView = new OutputParam<View>(ParamInfos.View);

        internal (View drawing, double angle) GetInputValues()
        {
            return (_inDrawing.Value,
                _inAngle.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, View view)
        {
            _outView.Value = view;

            return SetOutput(DA);
        }
    }
}
