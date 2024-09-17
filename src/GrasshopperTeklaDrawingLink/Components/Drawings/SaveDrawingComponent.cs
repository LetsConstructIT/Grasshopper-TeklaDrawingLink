using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class SaveDrawingComponent : TeklaComponentBaseNew<SaveDrawingCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.SaveDrawing;

        public SaveDrawingComponent() : base(ComponentInfos.SaveDrawingComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawing = _command.GetInputValues();

            var result = DrawingInteractor.DrawingHandler.SaveActiveDrawing();

            _command.SetOutputValues(DA, result);
        }
    }

    public class SaveDrawingCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);

        private readonly OutputParam<bool> _outResult = new OutputParam<bool>(ParamInfos.DrawingSaveResult);

        internal Drawing GetInputValues()
        {
            return _inDrawing.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, bool result)
        {
            _outResult.Value = result;

            return SetOutput(DA);
        }
    }
}
