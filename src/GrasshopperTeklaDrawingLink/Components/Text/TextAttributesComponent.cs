using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text
{
    public class TextAttributesComponent : TeklaComponentBaseNew<TextAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public TextAttributesComponent() : base(ComponentInfos.TextAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (fontAttributes, frameType, frameColor, arrowAttributes, backgroundTransparency, angle, rulerWidth, attributesName) = _command.GetInputValues();

            var textAttributes = new TSD.Text.TextAttributes(attributesName);

            if (fontAttributes != null)
                textAttributes.Font = fontAttributes;

            if (frameType != null)
                textAttributes.Frame = new Frame(frameType.Value, frameColor);

            if (arrowAttributes != null)
                textAttributes.ArrowHead = arrowAttributes;

            if (backgroundTransparency != null)
                textAttributes.TransparentBackground = backgroundTransparency.Value;

            if (angle != null)
                textAttributes.Angle = angle.Value;

            if (rulerWidth != null)
                textAttributes.RulerWidth = rulerWidth.Value;

            _command.SetOutputValues(DA, textAttributes);
        }
    }
    public class TextAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<FontAttributes> _inFontAttributes = new InputOptionalParam<FontAttributes>(ParamInfos.FontAttributes);
        private readonly InputOptionalStructParam<FrameTypes> _inFrameType = new InputOptionalStructParam<FrameTypes>(ParamInfos.FrameType);
        private readonly InputOptionalStructParam<DrawingColors> _inFrameColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor, DrawingColors.Black);
        private readonly InputOptionalParam<ArrowheadAttributes> _inArrowAttributes = new InputOptionalParam<ArrowheadAttributes>(ParamInfos.ArrowAttribute);
        private readonly InputOptionalStructParam<bool> _inBackgroundTransparency = new InputOptionalStructParam<bool>(ParamInfos.BackgroundTransparency);
        private readonly InputOptionalStructParam<double> _inAngle = new InputOptionalStructParam<double>(ParamInfos.Angle);
        private readonly InputOptionalStructParam<double> _inTextRulerWidth = new InputOptionalStructParam<double>(ParamInfos.TextRulerWidth);
        private readonly InputOptionalParam<string> _inAttributesName = new InputOptionalParam<string>(ParamInfos.Attributes, "standard");

        private readonly OutputParam<TSD.Text.TextAttributes> _outAttributes = new OutputParam<TSD.Text.TextAttributes>(ParamInfos.TextAttributes);

        internal (FontAttributes? fontAttributes, FrameTypes? frameType, DrawingColors frameColor, ArrowheadAttributes? arrowAttributes, bool? backgroundTransparency, double? angle, double? rulerWidth, string? attributesName) GetInputValues()
        {
            return (
                _inFontAttributes.ValueProvidedByUser ? _inFontAttributes.Value : null,
                _inFrameType.ValueProvidedByUser ? _inFrameType.Value : new FrameTypes?(),
                _inFrameColor.Value,
                _inArrowAttributes.ValueProvidedByUser ? _inArrowAttributes.Value : null,
                _inBackgroundTransparency.ValueProvidedByUser ? _inBackgroundTransparency.Value : new bool?(),
                _inAngle.ValueProvidedByUser ? _inAngle.Value : new double?(),
                _inTextRulerWidth.ValueProvidedByUser ? _inTextRulerWidth.Value : new double?(),
                _inAttributesName.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, TSD.Text.TextAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
