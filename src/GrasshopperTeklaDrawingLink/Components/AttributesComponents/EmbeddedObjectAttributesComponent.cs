using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class EmbeddedObjectAttributesComponent : TeklaComponentBaseNew<EmbeddedObjectAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Resources.DwgdxfAttributes;

        public EmbeddedObjectAttributesComponent() : base(ComponentInfos.EmbeddedObjectAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (dwgAttributes, fileName, scaling, scaleX, scaleY, frame) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                dwgAttributes.LoadAttributes(fileName);

            if (scaling.HasValue)
                dwgAttributes.Scaling = scaling.Value;

            if (scaleX.HasValue)
                dwgAttributes.XScale = scaleX.Value;

            if (scaleY.HasValue)
                dwgAttributes.YScale = scaleY.Value;

            if (frame != null)
                dwgAttributes.Frame = new EmbeddedObjectFrame(frame.Type, frame.Color);

            _command.SetOutputValues(DA, dwgAttributes);
        }
    }

    public class EmbeddedObjectAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<EmbeddedObjectAttributes> _inDwgAttributes = new InputOptionalParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);

        private readonly InputOptionalStructParam<EmbeddedObjectScalingOptions> _inScaling = new InputOptionalStructParam<EmbeddedObjectScalingOptions>(ParamInfos.DwgScaling);
        private readonly InputOptionalStructParam<double> _inXScale = new InputOptionalStructParam<double>(ParamInfos.XScale);
        private readonly InputOptionalStructParam<double> _inYScale = new InputOptionalStructParam<double>(ParamInfos.YScale);
        private readonly InputOptionalParam<LineTypeAttributes> _inFrame = new InputOptionalParam<LineTypeAttributes>(ParamInfos.Frame);

        private readonly OutputParam<EmbeddedObjectAttributes> _outAttributes = new OutputParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);

        internal (EmbeddedObjectAttributes dwgAttributes,
            string fileName,
            EmbeddedObjectScalingOptions? scaling,
            double? xScale,
            double? yScale,
            LineTypeAttributes frame)
            GetInputValues()
        {
            return (
                _inDwgAttributes.Value ?? new EmbeddedObjectAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inScaling.GetValueFromUserOrNull(),
                _inXScale.GetValueFromUserOrNull(),
                _inYScale.GetValueFromUserOrNull(),
                _inFrame.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, EmbeddedObjectAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
