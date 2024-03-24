using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class DeleteDrawingComponent : TeklaComponentBaseNew<DeleteDrawingCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.DeleteDrawing;

        public DeleteDrawingComponent() : base(ComponentInfos.DeleteDrawingComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawing = _command.GetInputValues();

            var result = drawing.Delete();

            _command.SetOutputValues(DA, result);
        }
    }

    public class DeleteDrawingCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);

        private readonly OutputParam<bool> _outResult = new OutputParam<bool>(ParamInfos.DrawingDeleteResult);

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
