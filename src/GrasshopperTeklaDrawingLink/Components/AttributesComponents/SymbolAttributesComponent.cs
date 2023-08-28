using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class SymbolAttributesComponent : TeklaComponentBaseNew<SymbolAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override Bitmap Icon => Resources.SymbolAttributes;

        public SymbolAttributesComponent() : base(ComponentInfos.SymbolAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (SymbolAttributes symbolAttributes, string fileName, SymbolInfo symbolInfo, DrawingColors? color, double? height, double? angle, Frame? frame) = _command.GetInputValues();

            var attributes = symbolAttributes.Attributes;
            if (!string.IsNullOrEmpty(fileName))
                attributes.LoadAttributes(fileName);

            if (color.HasValue)
                attributes.Color = color.Value;

            if (height.HasValue)
                attributes.Height = height.Value;

            if (angle.HasValue)
                attributes.Angle = angle.Value;

            if (frame != null)
                attributes.Frame = frame;

            if (symbolInfo != null)
                symbolAttributes.SymbolInfo = symbolInfo;

            _command.SetOutputValues(DA, symbolAttributes);
        }
    }

    public class SymbolAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<SymbolAttributes> _inSymbolAttributes = new InputOptionalParam<SymbolAttributes>(ParamInfos.SymbolAtributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<SymbolInfo> _inInfo = new InputOptionalParam<SymbolInfo>(ParamInfos.SymbolSelection);
        private readonly InputOptionalStructParam<DrawingColors> _inColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor);
        private readonly InputOptionalStructParam<double> _inHeight = new InputOptionalStructParam<double>(ParamInfos.Height);
        private readonly InputOptionalStructParam<double> _inAngle = new InputOptionalStructParam<double>(ParamInfos.Angle);
        private readonly InputOptionalParam<Frame> _inFrame = new InputOptionalParam<Frame>(ParamInfos.FrameAtributes);

        private readonly OutputParam<SymbolAttributes> _outAttributes = new OutputParam<SymbolAttributes>(ParamInfos.SymbolAtributes);

        internal (SymbolAttributes attributes, string fileName, SymbolInfo? symbolInfo, DrawingColors? color, double? height, double? angle, Frame? frame) GetInputValues()
        {
            return (
                _inSymbolAttributes.Value ?? new SymbolAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inInfo.GetValueFromUserOrNull(),
                _inColor.GetValueFromUserOrNull(),
                _inHeight.GetValueFromUserOrNull(),
                _inAngle.GetValueFromUserOrNull(),
                _inFrame.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, SymbolAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
