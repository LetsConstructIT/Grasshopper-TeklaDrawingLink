using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using Rhino.Geometry;
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
            drawing.Select();

            var sheetSize = drawing.Layout.SheetSize;
            var width = sheetSize.Width;
            var height = sheetSize.Height;
            var rectangle = new Rectangle3d(Plane.WorldXY, width, height);

            _command.SetOutputValues(DA, width, height, rectangle);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }

    public class GetDrawingSizeCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly OutputParam<double> _outWidth = new OutputParam<double>(new GH_InstanceDescription("Width", "W", "Drawing width", "", ""));
        private readonly OutputParam<double> _outHeight = new OutputParam<double>(new GH_InstanceDescription("Height", "H", "Drawing height", "", ""));
        private readonly OutputParam<Rectangle3d> _outRectangle = new OutputParam<Rectangle3d>(new GH_InstanceDescription("Rectangle", "R", "Drawing rectangle", "", ""));

        internal Drawing GetInputValues()
        {
            return _inDrawing.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, double width, double height, Rectangle3d rectangle)
        {
            _outWidth.Value = width;
            _outHeight.Value = height;
            _outRectangle.Value = rectangle;

            return SetOutput(DA);
        }
    }
}
