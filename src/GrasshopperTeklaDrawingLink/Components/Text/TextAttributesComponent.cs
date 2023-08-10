using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Tekla.Structures.Drawing;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text {
    public class TextAttributesComponent : TeklaComponentBase {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.LineTypeAttributes;

        public TextAttributesComponent()
            : base(ComponentInfos.TextAttributesComponent) {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            List<int> indeces = new List<int> {
                pManager.AddParameter(new FontAttributesParam(ParamInfos.FontAttributes, GH_ParamAccess.item)),
                pManager.AddParameter(new EnumParam<FrameTypes>(ParamInfos.FrameType, GH_ParamAccess.item)),
                pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item))
            };
            SetParametersAsOptional(pManager, indeces);
            AddBooleanParameter(pManager, ParamInfos.BackgroundTransparency, GH_ParamAccess.item, true);
            AddNumberParameter(pManager, ParamInfos.Angle, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            string atrFileName = null;

            DA.GetData(ParamInfos.Attributes.Name, ref atrFileName);

            var textAttributes = new TSD.Text.TextAttributes();

            if(atrFileName!=null) {
                textAttributes=new TSD.Text.TextAttributes(atrFileName);
            }

            var transparency = new bool();
            DA.GetData(ParamInfos.BackgroundTransparency.Name, ref transparency);

            var angle = new double();
            DA.GetData(ParamInfos.Angle.Name, ref angle);

            object frameType = null;
            object frameColor = null;

            DA.GetData(ParamInfos.FrameType.Name, ref frameType);
            DA.GetData(ParamInfos.DrawingColor.Name, ref frameColor);

            if(frameType!=null) {
                var frameTypeEnumValue = EnumHelpers.ObjectToEnumValue<FrameTypes>(frameType);
                if(frameTypeEnumValue.HasValue) {
                    textAttributes.Frame=new Frame(frameTypeEnumValue.Value, DrawingColors.Black);
                }
            }
            if(frameColor!=null) {
                var frameColorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(frameColor);
                if(frameColorEnumValue.HasValue) {
                    textAttributes.Frame=new Frame(FrameTypes.None, frameColorEnumValue.Value);
                }
            }
            if(frameType!=null&&frameColor!=null) {
                var frameTypeEnumValue = EnumHelpers.ObjectToEnumValue<FrameTypes>(frameType);
                var frameColorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(frameColor);
                if(frameTypeEnumValue.HasValue&&frameColorEnumValue.HasValue) {
                    textAttributes.Frame=new Frame(frameTypeEnumValue.Value, frameColorEnumValue.Value);
                }
            }

            FontAttributesGoo font = new FontAttributesGoo();
            DA.GetData(ParamInfos.FontAttributes.Name, ref font);
            font.Value=font.Value??new FontAttributes();
            textAttributes.Font=font.Value;
            textAttributes.Angle=angle;
            textAttributes.TransparentBackground=transparency;
            DA.SetData(ParamInfos.TextAttributes.Name, new TextAttributesGoo(textAttributes));
        }


    }
}
