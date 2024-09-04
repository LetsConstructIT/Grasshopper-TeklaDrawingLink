using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
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

            lineAttributes = lineAttributes.Copy();
            if (!string.IsNullOrEmpty(fileName))
                lineAttributes.LoadAttributes(fileName);

            if (lineType != null)
                lineAttributes.Line = lineType;

            if (hatch != null)
                lineAttributes.Hatch = hatch.ToGraphicObjectHatch();

            _command.SetOutputValues(DA, lineAttributes);
        }
    }

    public class PolygonAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Polygon.PolygonAttributes> _inAttributes = new InputOptionalParam<Polygon.PolygonAttributes>(ParamInfos.PolygonAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes);
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
