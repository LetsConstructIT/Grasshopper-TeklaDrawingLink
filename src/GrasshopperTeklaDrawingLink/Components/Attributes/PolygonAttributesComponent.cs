using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System;
using System.Drawing;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class PolygonAttributesComponent : TeklaComponentBaseNew<PolygonAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.PolygonAttributes;

        public PolygonAttributesComponent() : base(ComponentInfos.PolygonAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (lineAttributes, fileName, lineType, hatch) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                lineAttributes.LoadAttributes(fileName);

            if (lineType != null)
                lineAttributes.Line = lineType;

            if (hatch != null)
            {
                var hatchAttributes = (GraphicObjectHatchAttributes)typeof(GraphicObjectHatchAttributes).GetConstructor(
                  BindingFlags.NonPublic | BindingFlags.Instance,
                  null, Type.EmptyTypes, null).Invoke(null);

                hatchAttributes.Name = hatch.Name;
                hatchAttributes.Color = hatch.Color;
                hatchAttributes.BackgroundColor = hatch.BackgroundColor;
                hatchAttributes.DrawBackgroundColor = hatch.DrawBackgroundColor;
                hatchAttributes.ScaleX = hatch.ScaleX;
                hatchAttributes.ScaleY = hatch.ScaleY;

                lineAttributes.Hatch = hatchAttributes;
            }

            _command.SetOutputValues(DA, lineAttributes);
        }
    }

    public class PolygonAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Polygon.PolygonAttributes> _inAttributes = new InputOptionalParam<Polygon.PolygonAttributes>(ParamInfos.PolygonAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes2);
        private readonly InputOptionalParam<ModelObjectHatchAttributes> _inHatch = new InputOptionalParam<ModelObjectHatchAttributes>(ParamInfos.HatchAttributes);

        private readonly OutputParam<Polygon.PolygonAttributes> _outAttributes = new OutputParam<Polygon.PolygonAttributes>(ParamInfos.PolygonAttributes);

        internal (Polygon.PolygonAttributes attributes, string fileName, LineTypeAttributes? lineType, ModelObjectHatchAttributes? hatch) GetInputValues()
        {
            return (
                _inAttributes.Value ?? new Polygon.PolygonAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inHatch.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Polygon.PolygonAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
