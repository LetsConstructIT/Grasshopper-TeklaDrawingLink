using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class PolylineAttributesComponent : TeklaComponentBaseNew<PolylineAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.PolylineAttributes;

        public PolylineAttributesComponent() : base(ComponentInfos.PolylineAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (lineAttributes, fileName, lineType, arrowhead) = _command.GetInputValues();

            lineAttributes = lineAttributes.Copy();
            if (!string.IsNullOrEmpty(fileName))
                lineAttributes.LoadAttributes(fileName);

            if (lineType != null)
                lineAttributes.Line = lineType;

            if (arrowhead != null)
                lineAttributes.Arrowhead = arrowhead;

            _command.SetOutputValues(DA, lineAttributes);
        }
    }

    public class PolylineAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Polyline.PolylineAttributes> _inLineAttributes = new InputOptionalParam<Polyline.PolylineAttributes>(ParamInfos.PolylineAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes);
        private readonly InputOptionalParam<ArrowheadAttributes> _inArrowhead = new InputOptionalParam<ArrowheadAttributes>(ParamInfos.ArrowAttributes);

        private readonly OutputParam<Polyline.PolylineAttributes> _outAttributes = new OutputParam<Polyline.PolylineAttributes>(ParamInfos.PolylineAttributes);

        internal (Polyline.PolylineAttributes lineAttributes, string fileName, LineTypeAttributes? lineType, ArrowheadAttributes? arrowhead) GetInputValues()
        {
            return (
                _inLineAttributes.Value ?? new Polyline.PolylineAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inArrowhead.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Polyline.PolylineAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
