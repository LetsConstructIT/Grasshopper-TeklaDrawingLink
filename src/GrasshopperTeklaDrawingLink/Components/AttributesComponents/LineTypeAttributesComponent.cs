using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class LineTypeAttributesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.LineTypeAttributes;

        public LineTypeAttributesComponent()
            : base(ComponentInfos.LineTypeAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new EnumParam<LineTypesEnum>(ParamInfos.LineType, GH_ParamAccess.item));
            pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item));

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new LineTypeAttributesParam(ParamInfos.LineTypeAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var lineTypeAttributes = new LineTypeAttributes();

            object lineType = null;
            DA.GetData(ParamInfos.LineType.Name, ref lineType);
            if (lineType != null)
            {
                var lineTypeValue = EnumHelpers.ObjectToEnumValue<LineTypesEnum>(lineType);
                lineTypeAttributes.Type = GetLineTypeBasedOnEnum(lineTypeValue);
            }

            object color = null;
            DA.GetData(ParamInfos.DrawingColor.Name, ref color);
            if (color != null)
            {
                var colorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(color);
                if (colorEnumValue.HasValue)
                    lineTypeAttributes.Color = colorEnumValue.Value;
            }

            DA.SetData(ParamInfos.LineTypeAttributes.Name, new LineTypeAttributesGoo(lineTypeAttributes));
        }

        private LineTypes GetLineTypeBasedOnEnum(LineTypesEnum? lineTypeValue)
        {
            if (!lineTypeValue.HasValue)
                return LineTypes.SolidLine;

            switch (lineTypeValue.Value)
            {
                case LineTypesEnum.SolidLine:
                    return LineTypes.SolidLine;
                case LineTypesEnum.DashedLine:
                    return LineTypes.DashedLine;
                case LineTypesEnum.SlashedLine:
                    return LineTypes.SlashedLine;
                case LineTypesEnum.DashDot:
                    return LineTypes.DashDot;
                case LineTypesEnum.DottedLine:
                    return LineTypes.DottedLine;
                case LineTypesEnum.DashDoubleDot:
                    return LineTypes.DashDoubleDot;
                case LineTypesEnum.SlashDash:
                    return LineTypes.SlashDash;
                default:
                    return LineTypes.SolidLine;
            }
        }
    }
}
