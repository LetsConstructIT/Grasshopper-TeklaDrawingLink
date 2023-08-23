using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CloseDrawingComponent : TeklaComponentBaseNew<CloseDrawingCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.CloseDrawing;

        public CloseDrawingComponent() : base(ComponentInfos.CloseDrawingComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (drawing, save) = _command.GetInputValues();

            var result = DrawingInteractor.DrawingHandler.CloseActiveDrawing(save);

            _command.SetOutputValues(DA, result);
        }
    }

    public class CloseDrawingCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly InputOptionalStructParam<bool> _inSave = new InputOptionalStructParam<bool>(ParamInfos.SaveDrawing, true);

        private readonly OutputParam<bool> _outResult = new OutputParam<bool>(ParamInfos.DrawingSaveResult);

        internal (Drawing drawing, bool save) GetInputValues()
        {
            return (
                _inDrawing.Value,
                _inSave.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, bool result)
        {
            _outResult.Value = result;

            return SetOutput(DA);
        }
    }
}
