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
            pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item));
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
            if(font!=null) {
                Grasshopper.Kernel.Types.GH_String gH_String = font as Grasshopper.Kernel.Types.GH_String;
                if(gH_String==null) {
                    fontAttributes.Name="Arial Narrow";
                }
                else {
                    fontAttributes.Name=gH_String.Value;
                }
            }
            else {
                fontAttributes.Name="Arial Narrow";
            }

            object size = null;
            DA.GetData(ParamInfos.FontSize.Name, ref size);
            if(size!=null) {
                Grasshopper.Kernel.Types.GH_Number gH_Number = size as Grasshopper.Kernel.Types.GH_Number;

                var fontValue = gH_Number.Value;
                if(fontValue==0) {
                    fontAttributes.Height=2.5;
                }
                else {
                    fontAttributes.Height=fontValue;
                }
            }
            else {
                fontAttributes.Height=2.5;
            }

            object weight = null;
            DA.GetData(ParamInfos.FontWeight.Name, ref weight);
            if(weight!=null) {
                Grasshopper.Kernel.Types.GH_Boolean gH_Boolean = weight as Grasshopper.Kernel.Types.GH_Boolean;
                if(gH_Boolean==null) {
                    fontAttributes.Bold=false;
                }
                else {
                    fontAttributes.Bold=gH_Boolean.Value;
                }
            }
            else {
                fontAttributes.Bold=false;
            }

            object italic = null;
            DA.GetData(ParamInfos.FontItalic.Name, ref italic);
            if(italic!=null) {
                Grasshopper.Kernel.Types.GH_Boolean gH_Boolean = italic as Grasshopper.Kernel.Types.GH_Boolean;
                if(gH_Boolean==null) {
                    fontAttributes.Italic=false;
                }
                else {
                    fontAttributes.Italic=gH_Boolean.Value;
                }
            }
            else {
                fontAttributes.Italic=false;
            }



            DA.SetData(ParamInfos.FontAttributes.Name, new FontAttributesGoo(fontAttributes));
        }


    }
}
