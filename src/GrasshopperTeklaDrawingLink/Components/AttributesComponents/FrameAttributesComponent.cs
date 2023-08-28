using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class FrameAttributesComponent : TeklaComponentBaseNew<FrameAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.FrameAttributes;

        public FrameAttributesComponent() : base(ComponentInfos.FrameAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (head, color) = _command.GetInputValues();

            var frame = new Frame(head, color);

            _command.SetOutputValues(DA, frame);
        }
    }

    public class FrameAttributesCommand : CommandBase
    {
        private readonly InputOptionalStructParam<FrameTypes> _inFrameType = new InputOptionalStructParam<FrameTypes>(ParamInfos.FrameType, FrameTypes.None);
        private readonly InputOptionalStructParam<DrawingColors> _inDrawingColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor, DrawingColors.Black);

        private readonly OutputParam<Frame> _outAttributes = new OutputParam<Frame>(ParamInfos.FrameAtributes);

        internal (FrameTypes type, DrawingColors color) GetInputValues()
        {
            return (
                _inFrameType.Value,
                _inDrawingColor.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Frame attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
