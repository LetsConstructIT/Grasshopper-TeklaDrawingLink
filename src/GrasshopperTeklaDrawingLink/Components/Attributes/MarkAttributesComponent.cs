using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class MarkAttributesComponent : TeklaComponentBaseNew<MarkAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.MarkAttributes;

        public MarkAttributesComponent() : base(ComponentInfos.MarkAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (markAttributes, fileName, modelObject, frame, arrowAttributes, backgroundTransparency) = _command.GetInputValues();

            if (markAttributes is null)
            {
                if (modelObject is null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Drawing object is obligatory to properly read marks' content");
                    return;
                }
                modelObject.Select();
                markAttributes = new Mark.MarkAttributes(modelObject);
            }
            else
                markAttributes = markAttributes.Copy();

            if (!string.IsNullOrEmpty(fileName))
                markAttributes.LoadAttributes(fileName);

            if (frame != null)
                markAttributes.Frame = frame;

            if (arrowAttributes != null)
                markAttributes.ArrowHead = arrowAttributes;

            if (backgroundTransparency != null)
                markAttributes.TransparentBackground = backgroundTransparency.Value;

            _command.SetOutputValues(DA, markAttributes);
        }
    }
    public class MarkAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Mark.MarkAttributes> _inMarkAttributes = new InputOptionalParam<Mark.MarkAttributes>(ParamInfos.MarkAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.AttributesForMarkAttributes);
        private readonly InputOptionalParam<ModelObject> _inModel = new InputOptionalParam<ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputOptionalParam<Frame> _inFrameType = new InputOptionalParam<Frame>(ParamInfos.FrameAtributes);
        private readonly InputOptionalParam<ArrowheadAttributes> _inArrowAttributes = new InputOptionalParam<ArrowheadAttributes>(ParamInfos.ArrowAttributes);
        private readonly InputOptionalStructParam<bool> _inBackgroundTransparency = new InputOptionalStructParam<bool>(ParamInfos.BackgroundTransparency);

        private readonly OutputParam<Mark.MarkAttributes> _outAttributes = new OutputParam<Mark.MarkAttributes>(ParamInfos.MarkAttributes);

        internal (Mark.MarkAttributes? textAttributes, string? attributesName, ModelObject? modelObject, Frame? frame, ArrowheadAttributes? arrowAttributes, bool? backgroundTransparency) GetInputValues()
        {
            return (
                _inMarkAttributes.GetValueFromUserOrNull(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inModel.GetValueFromUserOrNull(),
                _inFrameType.GetValueFromUserOrNull(),
                _inArrowAttributes.GetValueFromUserOrNull(),
                _inBackgroundTransparency.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Mark.MarkAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
