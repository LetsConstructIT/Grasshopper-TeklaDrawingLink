using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class CircleAttributesComponent : TeklaComponentBaseNew<CircleAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.CircleAttributes;

        public CircleAttributesComponent() : base(ComponentInfos.CircleAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (circleAttributes, fileName, lineType, hatchAttributes, behindModelObjects) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                circleAttributes.LoadAttributes(fileName);

            if (lineType != null)
                circleAttributes.Line = lineType;

            if (hatchAttributes != null)
                circleAttributes.Hatch = hatchAttributes.ToGraphicObjectHatch();

            if (behindModelObjects.HasValue)
                circleAttributes.BehindModelObjects = behindModelObjects.Value;

            _command.SetOutputValues(DA, circleAttributes);
        }
    }

    public class CircleAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Circle.CircleAttributes> _inLineAttributes = new InputOptionalParam<Circle.CircleAttributes>(ParamInfos.CircleAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes2);
        private readonly InputOptionalParam<ModelObjectHatchAttributes> _inHatchAttributes = new InputOptionalParam<ModelObjectHatchAttributes>(ParamInfos.HatchAttributes);
        private readonly InputOptionalStructParam<bool> _inBehindModelObject = new InputOptionalStructParam<bool>(ParamInfos.BehindModelObject);

        private readonly OutputParam<Circle.CircleAttributes> _outAttributes = new OutputParam<Circle.CircleAttributes>(ParamInfos.CircleAttributes);

        internal (Circle.CircleAttributes circleAttributes, string fileName, LineTypeAttributes? lineTypeAttributes, ModelObjectHatchAttributes? hatchAttributes, bool? arrowhead) GetInputValues()
        {
            return (
                _inLineAttributes.Value ?? new Circle.CircleAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inHatchAttributes.GetValueFromUserOrNull(),
                _inBehindModelObject.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Circle.CircleAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
