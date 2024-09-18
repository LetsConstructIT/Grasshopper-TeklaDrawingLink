using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class ArcAttributesComponent : TeklaComponentBaseNew<ArcAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.ArcAttributes;

        public ArcAttributesComponent() : base(ComponentInfos.ArcAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (arcAttributes, fileName, lineType, arrowhead) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                arcAttributes.LoadAttributes(fileName);

            if (lineType != null)
                arcAttributes.Line = lineType;

            if (arrowhead != null)
                arcAttributes.Arrowhead = arrowhead;

            _command.SetOutputValues(DA, arcAttributes);
        }
    }

    public class ArcAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Arc.ArcAttributes> _inArcAttributes = new InputOptionalParam<Arc.ArcAttributes>(ParamInfos.ArcAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes);
        private readonly InputOptionalParam<ArrowheadAttributes> _inArrowhead = new InputOptionalParam<ArrowheadAttributes>(ParamInfos.ArrowAttributes);

        private readonly OutputParam<Arc.ArcAttributes> _outAttributes = new OutputParam<Arc.ArcAttributes>(ParamInfos.ArcAttributes);

        internal (Arc.ArcAttributes circleAttributes, string fileName, LineTypeAttributes? lineTypeAttributes, ArrowheadAttributes? arrowheadAttributes) GetInputValues()
        {
            return (
                _inArcAttributes.Value ?? new Arc.ArcAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inArrowhead.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Arc.ArcAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
