using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingPropertiesComponent : TeklaComponentBaseNew<GetDrawingPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingProperties;

        public GetDrawingPropertiesComponent() : base(ComponentInfos.GetDrawingPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawing = _command.GetInputValues();

            var sheetSize = drawing.Layout.SheetSize;

            _command.SetOutputValues(DA, drawing.Name, drawing.Title1, drawing.Title2, drawing.Title3, sheetSize.Width, sheetSize.Height);
        }
    }

    public class GetDrawingPropertiesCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly OutputParam<string> _outName = new OutputParam<string>(new GH_InstanceDescription("Name", "N", "Drawing name", "", ""));
        private readonly OutputParam<string> _outTitle1 = new OutputParam<string>(new GH_InstanceDescription("Title1", "T1", "Drawing title 1", "", ""));
        private readonly OutputParam<string> _outTitle2 = new OutputParam<string>(new GH_InstanceDescription("Title2", "T2", "Drawing title 2", "", ""));
        private readonly OutputParam<string> _outTitle3 = new OutputParam<string>(new GH_InstanceDescription("Title3", "T3", "Drawing title 3", "", ""));
        private readonly OutputParam<double> _outWidth = new OutputParam<double>(new GH_InstanceDescription("Width", "W", "Drawing width", "", ""));
        private readonly OutputParam<double> _outHeight = new OutputParam<double>(new GH_InstanceDescription("Height", "H", "Drawing height", "", ""));

        internal Drawing GetInputValues()
        {
            return _inDrawing.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string name, string title1, string title2, string title3, double width, double height)
        {
            _outName.Value = name;
            _outTitle1.Value = title1;
            _outTitle2.Value = title2;
            _outTitle3.Value = title3;
            _outWidth.Value = width;
            _outHeight.Value = height;

            return SetOutput(DA);
        }
    }
}
