using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingSizeComponent : TeklaComponentBaseNew<GetDrawingSizeCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingSize;

        public GetDrawingSizeComponent() : base(ComponentInfos.GetDrawingSizeComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawing = _command.GetInputValues();

            var sheetSize = drawing.Layout.SheetSize;

            _command.SetOutputValues(DA, sheetSize.Width, sheetSize.Height);
        }
    }

    public class GetDrawingSizeCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly OutputParam<double> _outWidth = new OutputParam<double>(new GH_InstanceDescription("Width", "W", "Drawing width", "", ""));
        private readonly OutputParam<double> _outHeight = new OutputParam<double>(new GH_InstanceDescription("Height", "H", "Drawing height", "", ""));

        internal Drawing GetInputValues()
        {
            return _inDrawing.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, double width, double height)
        {
            _outWidth.Value = width;
            _outHeight.Value = height;

            return SetOutput(DA);
        }
    }
}
