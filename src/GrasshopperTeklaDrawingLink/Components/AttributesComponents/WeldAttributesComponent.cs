using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class WeldAttributesComponent : TeklaComponentBaseNew<WeldAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.WeldAttributes;
        public WeldAttributesComponent() : base(ComponentInfos.WeldAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (weldAttributes, fileName, representation, visibleLines, hiddenLines, drawHiddenLines, drawOwnHiddenLines) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                weldAttributes.LoadAttributes(fileName);

            if (representation.HasValue)
                weldAttributes.Representation = representation.Value;

            if (visibleLines != null)
                weldAttributes.VisibleLines = visibleLines;

            if (hiddenLines != null)
                weldAttributes.HiddenLines = hiddenLines;

            if (drawHiddenLines.HasValue)
                weldAttributes.DrawHiddenLines = drawHiddenLines.Value;

            if (drawOwnHiddenLines.HasValue)
                weldAttributes.DrawOwnHiddenLines = drawOwnHiddenLines.Value;

            _command.SetOutputValues(DA, weldAttributes);
        }
    }

    public class WeldAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Weld.WeldAttributes> _inWeldAttributes = new InputOptionalParam<Weld.WeldAttributes>(ParamInfos.WeldAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);

        private readonly InputOptionalStructParam<Weld.Representation> _inRepresentation = new InputOptionalStructParam<Weld.Representation>(ParamInfos.WeldRepresentation);
        private readonly InputOptionalParam<LineTypeAttributes> _inVisibleLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.VisibleLineTypeAttributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inHiddenLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);

        private readonly InputOptionalStructParam<bool> _inDrawHiddenLines = new InputOptionalStructParam<bool>(ParamInfos.DrawHiddenLines);
        private readonly InputOptionalStructParam<bool> _inDrawOwnHiddenLines = new InputOptionalStructParam<bool>(ParamInfos.DrawOwnHiddenLines);

        private readonly OutputParam<Weld.WeldAttributes> _outAttributes = new OutputParam<Weld.WeldAttributes>(ParamInfos.WeldAttributes);

        internal (Weld.WeldAttributes weldAttributes,
            string fileName,
            Weld.Representation? representation,
            LineTypeAttributes? visibleLines,
            LineTypeAttributes? hiddenLines,
            bool? axis,
            bool? hole)
            GetInputValues()
        {
            return (
                _inWeldAttributes.Value ?? new Weld.WeldAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inRepresentation.GetValueFromUserOrNull(),
                _inVisibleLines.GetValueFromUserOrNull(),
                _inHiddenLines.GetValueFromUserOrNull(),
                _inDrawHiddenLines.GetValueFromUserOrNull(),
                _inDrawOwnHiddenLines.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Weld.WeldAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
