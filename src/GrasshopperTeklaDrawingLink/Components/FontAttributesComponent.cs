using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class FontAttributesComponent : TeklaComponentBaseNew<FontAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.FontAttributes;

        public FontAttributesComponent() : base(ComponentInfos.FontAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (color, fontFamily, fontSize, weight, italic) = _command.GetInputValues();

            var fontAttributes = new FontAttributes()
            {
                Color = color,
                Name = fontFamily,
                Height = fontSize,
                Bold = weight,
                Italic = italic
            };

            _command.SetOutputValues(DA, fontAttributes);
        }
    }

    public class FontAttributesCommand : CommandBase
    {
        private readonly InputOptionalStructParam<DrawingColors> _inDrawingColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor, DrawingColors.Black);
        private readonly InputOptionalParam<string> _inFontFamily = new InputOptionalParam<string>(ParamInfos.FontFamily, "Arial");
        private readonly InputOptionalStructParam<double> _inFontSize = new InputOptionalStructParam<double>(ParamInfos.FontSize, 2.5);
        private readonly InputOptionalStructParam<bool> _inFontWeight = new InputOptionalStructParam<bool>(ParamInfos.FontWeight, false);
        private readonly InputOptionalStructParam<bool> _inFontItalic = new InputOptionalStructParam<bool>(ParamInfos.FontItalic, false);

        private readonly OutputParam<FontAttributes> _outAttributes = new OutputParam<FontAttributes>(ParamInfos.FontAttributes);

        internal (DrawingColors color, string fontFamily, double fontSize, bool weight, bool italic) GetInputValues()
        {
            return (
                _inDrawingColor.Value,
                _inFontFamily.Value,
                _inFontSize.Value,
                _inFontWeight.Value,
                _inFontItalic.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, FontAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
