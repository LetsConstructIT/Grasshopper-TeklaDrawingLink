using Grasshopper.Kernel;

using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using System.Collections.Generic;

using Tekla.Structures.Drawing;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text
{
    public class TextAttributesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public TextAttributesComponent()
            : base(ComponentInfos.TextAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            SetParametersAsOptional(pManager, new List<int> {
                pManager.AddParameter(new FontAttributesParam(ParamInfos.FontAttributes, GH_ParamAccess.item)),
                pManager.AddParameter(new EnumParam<FrameTypes>(ParamInfos.FrameType, GH_ParamAccess.item)),
                pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item)),
                pManager.AddParameter(new ArrowAttributesParam(ParamInfos.ArrowAttribute, GH_ParamAccess.item)),
                pManager.AddBooleanParameter(
                    ParamInfos.BackgroundTransparency.Name,
                    ParamInfos.BackgroundTransparency.NickName,
                    ParamInfos.BackgroundTransparency.Description,
                    GH_ParamAccess.item),
                pManager.AddNumberParameter(
                    ParamInfos.Angle.Name,
                    ParamInfos.Angle.NickName,
                    ParamInfos.Angle.Description,
                    GH_ParamAccess.item),
                pManager.AddNumberParameter(
                    ParamInfos.TextRulerWidth.Name,
                    ParamInfos.TextRulerWidth.NickName,
                    ParamInfos.TextRulerWidth.Description,
                    GH_ParamAccess.item)
            });

            pManager.AddTextParameter(
                 ParamInfos.Attributes.Name,
                 ParamInfos.Attributes.NickName,
                 ParamInfos.Attributes.Description,
                 GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string attributesFromFile = null;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFromFile);

            var transparency = new bool();
            DA.GetData(ParamInfos.BackgroundTransparency.Name, ref transparency);

            var angle = new double();
            DA.GetData(ParamInfos.Angle.Name, ref angle);

            var rulerWidth = new double();
            DA.GetData(ParamInfos.TextRulerWidth.Name, ref rulerWidth);

            var font = new FontAttributesGoo();
            DA.GetData(ParamInfos.FontAttributes.Name, ref font);


            var arrowheadAttributes = new ArrowAttributesGoo();
            DA.GetData(ParamInfos.ArrowAttribute.Name, ref arrowheadAttributes);


            var textAttributes = new TSD.Text.TextAttributes();

            if (attributesFromFile != null)
            {
                textAttributes = new TSD.Text.TextAttributes(attributesFromFile);
            }

            object frameType = null;
            object frameColor = null;
            DA.GetData(ParamInfos.FrameType.Name, ref frameType);
            if (frameType != null)
            {
                var frameTypeEnumValue = EnumHelpers.ObjectToEnumValue<FrameTypes>(frameType);
                if (frameTypeEnumValue.HasValue)
                {
                    textAttributes.Frame = new Frame(frameTypeEnumValue.Value, DrawingColors.Black);
                }
            }

            DA.GetData(ParamInfos.DrawingColor.Name, ref frameColor);
            if (frameColor != null)
            {
                var frameColorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(frameColor);
                if (frameColorEnumValue.HasValue)
                {
                    textAttributes.Frame = new Frame(FrameTypes.None, frameColorEnumValue.Value);
                }
            }


            if (frameType != null && frameColor != null)
            {
                var frameTypeEnumValue = EnumHelpers.ObjectToEnumValue<FrameTypes>(frameType);
                var frameColorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(frameColor);
                if (frameTypeEnumValue.HasValue && frameColorEnumValue.HasValue)
                {
                    textAttributes.Frame = new Frame(frameTypeEnumValue.Value, frameColorEnumValue.Value);
                }
            }

            textAttributes.Font = font?.Value ?? textAttributes.Font;


            textAttributes.Angle = (angle == 0.0) ? textAttributes.Angle : angle;
            textAttributes.RulerWidth = (rulerWidth == 0.0) ? textAttributes.RulerWidth : rulerWidth;
            textAttributes.TransparentBackground = (transparency == false) ? textAttributes.TransparentBackground : transparency;
            textAttributes.ArrowHead = (arrowheadAttributes.Value is null) ? textAttributes.ArrowHead : arrowheadAttributes.Value;

            DA.SetData(ParamInfos.TextAttributes.Name, new TextAttributesGoo(textAttributes));
        }
    }
}
