using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class LineAttributesComponent : TeklaComponentBaseNew<LineAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.LineAttributes;

        public LineAttributesComponent() : base(ComponentInfos.LineAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (lineAttributes, fileName, lineType, arrowhead) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                lineAttributes.LoadAttributes(fileName);

            if (lineType != null)
                lineAttributes.Line = lineType;

            if (arrowhead != null)
                lineAttributes.Arrowhead = arrowhead;

            _command.SetOutputValues(DA, lineAttributes);
        }
    }

    public class LineAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Line.LineAttributes> _inLineAttributes = new InputOptionalParam<Line.LineAttributes>(ParamInfos.LineAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes2);
        private readonly InputOptionalParam<ArrowheadAttributes> _inArrowhead = new InputOptionalParam<ArrowheadAttributes>(ParamInfos.ArrowAttributes);

        private readonly OutputParam<Line.LineAttributes> _outAttributes = new OutputParam<Line.LineAttributes>(ParamInfos.LineAttributes);

        internal (Line.LineAttributes lineAttributes, string fileName, LineTypeAttributes? lineType, ArrowheadAttributes? arrowhead) GetInputValues()
        {
            return (
                _inLineAttributes.Value ?? new Line.LineAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inArrowhead.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Line.LineAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
