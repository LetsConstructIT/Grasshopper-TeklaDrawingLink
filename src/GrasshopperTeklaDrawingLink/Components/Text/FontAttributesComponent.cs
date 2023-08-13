using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Tekla.Structures.Drawing;


namespace GTDrawingLink.Components.Text
{
    public class FontAttributesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public FontAttributesComponent()
            : base(ComponentInfos.FontAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            SetParametersAsOptional(pManager, new List<int> {
               pManager.AddParameter(new EnumParam<DrawingColors>(ParamInfos.DrawingColor, GH_ParamAccess.item)),

               pManager.AddTextParameter(
                   ParamInfos.FontFamily.Name,
                   ParamInfos.FontFamily.NickName,
                   ParamInfos.FontFamily.Description,
                   GH_ParamAccess.item),

               pManager.AddNumberParameter(
                   ParamInfos.FontSize.Name,
                   ParamInfos.FontSize.NickName,
                   ParamInfos.FontSize.Description,
                   GH_ParamAccess.item),

               pManager.AddBooleanParameter(
                   ParamInfos.FontWeight.Name,
                   ParamInfos.FontWeight.NickName,
                   ParamInfos.FontWeight.Description,
                   GH_ParamAccess.item),

               pManager.AddBooleanParameter(
                   ParamInfos.FontItalic.Name,
                   ParamInfos.FontItalic.NickName,
                   ParamInfos.FontItalic.Description,
                   GH_ParamAccess.item),
           }); 
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new FontAttributesParam(ParamInfos.FontAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object color = null;
            DA.GetData(ParamInfos.DrawingColor.Name, ref color);
            var colorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingColors>(color);

            string font = string.Empty;
            DA.GetData(ParamInfos.FontFamily.Name, ref font);
            font = font ?? "Arial";


            double height = 0.0;
            DA.GetData(ParamInfos.FontSize.Name, ref height);
          

            bool weight = false;
            DA.GetData(ParamInfos.FontWeight.Name, ref weight);

            bool italic = false;
            DA.GetData(ParamInfos.FontItalic.Name, ref italic);

            var fontAttributes = new FontAttributes
            {
                Color = colorEnumValue ?? DrawingColors.Black,
                Name = font,
                Height = height,
                Bold = weight,
                Italic = italic
            };

            DA.SetData(ParamInfos.FontAttributes.Name, new FontAttributesGoo(fontAttributes));
        }
    }
}
