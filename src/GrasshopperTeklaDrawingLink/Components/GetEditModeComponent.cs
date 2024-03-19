using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;

namespace GTDrawingLink.Components
{
    public class GetEditModeComponent : TeklaComponentBaseNew<GetEditModeCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override Bitmap Icon => Properties.Resources.GetEditMode;

        public GetEditModeComponent() : base(ComponentInfos.GetEditModeComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var trigger = _command.GetInputValues();
            if (!trigger)
                return;

            var mode = Tekla.Structures.DrawingInternal.Operation.GetEditMode();

            var model = mode == Tekla.Structures.DrawingInternal.EditMode.ModelEditMode;
            var drawing = mode == Tekla.Structures.DrawingInternal.EditMode.DrawingEditMode;

            _command.SetOutputValues(DA, model, drawing);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }

    public class GetEditModeCommand : CommandBase
    {
        private readonly InputStructParam<bool> _inTrigger = new InputStructParam<bool>(ParamInfos.BooleanToogle);

        private readonly OutputParam<bool> _outModel = new OutputParam<bool>(ParamInfos.ModelMode);
        private readonly OutputParam<bool> _outDrawing = new OutputParam<bool>(ParamInfos.DrawingMode);

        internal bool GetInputValues()
        {
            return _inTrigger.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, bool modelMode, bool drawingMode)
        {
            _outModel.Value = modelMode;
            _outDrawing.Value = drawingMode;

            return SetOutput(DA);
        }
    }
}
