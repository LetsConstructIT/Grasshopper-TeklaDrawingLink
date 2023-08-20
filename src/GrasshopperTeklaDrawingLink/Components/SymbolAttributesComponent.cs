using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text
{
    public class SymbolAttributesComponent : TeklaComponentBaseNew<SymbolAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.SymbolAttributes;

        public SymbolAttributesComponent() : base(ComponentInfos.SymbolAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Symbol.SymbolAttributes attributes, string fileName, DrawingColors? color, double? height, double? angle, Frame? frame) = _command.GetInputValues();

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

            _command.SetOutputValues(DA, attributes);
        }
    }

    public class SymbolAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Symbol.SymbolAttributes> _inSymbolAttributes = new InputOptionalParam<Symbol.SymbolAttributes>(ParamInfos.SymbolAtributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalStructParam<DrawingColors> _inColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor);
        private readonly InputOptionalStructParam<double> _inHeight = new InputOptionalStructParam<double>(ParamInfos.Height);
        private readonly InputOptionalStructParam<double> _inAngle = new InputOptionalStructParam<double>(ParamInfos.Angle);
        private readonly InputOptionalParam<Frame> _inFrame = new InputOptionalParam<Frame>(ParamInfos.FrameAtributes);

        private readonly OutputParam<Symbol.SymbolAttributes> _outAttributes = new OutputParam<Symbol.SymbolAttributes>(ParamInfos.SymbolAtributes);

        internal (Symbol.SymbolAttributes attributes, string fileName, DrawingColors? color, double? height, double? angle, Frame? frame) GetInputValues()
        {
            return (
                _inSymbolAttributes.Value ?? new Symbol.SymbolAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inColor.GetValueFromUserOrNull(),
                _inHeight.GetValueFromUserOrNull(),
                _inAngle.GetValueFromUserOrNull(),
                _inFrame.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Symbol.SymbolAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
