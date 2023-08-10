using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Tekla.Structures.Drawing;


namespace GTDrawingLink.Components.Text {
    public class FontAttributesComponent : TeklaComponentBase {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.Font;

        public FontAttributesComponent()
            : base(ComponentInfos.FontAttributesComponent) {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
           SetParametersAsOptional(pManager,new List<int> { pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item)) });
            AddTextParameter(pManager, ParamInfos.FontFamily, GH_ParamAccess.item, true);
            AddNumberParameter(pManager, ParamInfos.FontSize, GH_ParamAccess.item, true);
            AddBooleanParameter(pManager, ParamInfos.FontWeight, GH_ParamAccess.item, true);
            AddBooleanParameter(pManager, ParamInfos.FontItalic, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new FontAttributesParam(ParamInfos.FontAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            FontAttributes fontAttributes = new FontAttributes();

            object color = null;
            DA.GetData(ParamInfos.DrawingColor.Name, ref color);
            if(color!=null) {
                var colorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(color);
                if(colorEnumValue.HasValue)
                    fontAttributes.Color=colorEnumValue.Value;
            }

            object font = null;
            DA.GetData(ParamInfos.FontFamily.Name, ref font);
            if(font is null) {
                fontAttributes.Name="Arial";
            }
            else {
                if(!(font is Grasshopper.Kernel.Types.GH_String gH_String)) {
                    fontAttributes.Name="Arial";
                }
                else {
                    fontAttributes.Name=gH_String.Value;
                }
            }

            object size = null;
            DA.GetData(ParamInfos.FontSize.Name, ref size);
            if(size is null) {
                fontAttributes.Height=2.5;
            }
            else {
                if(size is Grasshopper.Kernel.Types.GH_Number gH_Number) {
                    var fontSize = gH_Number.Value;
                    if(fontSize==0) {
                        fontAttributes.Height=2.5;
                    }
                    else {
                        fontAttributes.Height=fontSize;
                    }
                }
            }

            object weight = null;
            DA.GetData(ParamInfos.FontWeight.Name, ref weight);
            if(weight is null) {
                fontAttributes.Bold=false;
            }
            else {
                if(!(weight is Grasshopper.Kernel.Types.GH_Boolean gH_Boolean)) {
                    fontAttributes.Bold=false;
                }
                else {
                    fontAttributes.Bold=gH_Boolean.Value;
                }
            }

            object italic = null;
            DA.GetData(ParamInfos.FontItalic.Name, ref italic);
            if(italic is null) {
                fontAttributes.Italic=false;
            }
            else {
                if(!(italic is Grasshopper.Kernel.Types.GH_Boolean gH_Boolean)) {
                    fontAttributes.Italic=false;
                }
                else {
                    fontAttributes.Italic=gH_Boolean.Value;
                }
            }
            DA.SetData(ParamInfos.FontAttributes.Name, new FontAttributesGoo(fontAttributes));
        }
    }
}
