using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class ModelObjectHatchAttributesComponent : TeklaComponentBase
    {
        private string _hatchNameParameter = "Name";
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ModelObjectHatchAttributes;

        public ModelObjectHatchAttributesComponent()
            : base(ComponentInfos.ModelObjectHatchAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter(_hatchNameParameter, "N", "Hatch name", GH_ParamAccess.item);
            pManager.AddParameter(new EnumParam<DrawingHatchColors>(ParamInfos.DrawingHatchColor, GH_ParamAccess.item));
            AddOptionalParameter(pManager, new EnumParam<DrawingHatchColors>(ParamInfos.DrawingBackgroundHatchColor, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ModelObjectHatchAttributesParam(ParamInfos.HatchAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var hatchAttributes = (ModelObjectHatchAttributes)typeof(ModelObjectHatchAttributes).GetConstructor(
                  BindingFlags.NonPublic | BindingFlags.Instance,
                  null, Type.EmptyTypes, null).Invoke(null);

            hatchAttributes.Name = "None";
            hatchAttributes.Color = DrawingHatchColors.Green;
            hatchAttributes.BackgroundColor = DrawingHatchColors.Green;
            hatchAttributes.DrawBackgroundColor = false;
            hatchAttributes.AutomaticScaling = true;
            hatchAttributes.ScaleX = 1.0;
            hatchAttributes.ScaleY = 1.0;

            var lineType = "None";
            DA.GetData(_hatchNameParameter, ref lineType);
            hatchAttributes.Name = lineType;

            object color = null;
            DA.GetData(ParamInfos.DrawingHatchColor.Name, ref color);
            if (color != null)
            {
                var colorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingHatchColors>(color);
                if (colorEnumValue.HasValue)
                    hatchAttributes.Color = colorEnumValue.Value;
            }

            object backgroundColor = null;
            DA.GetData(ParamInfos.DrawingBackgroundHatchColor.Name, ref backgroundColor);
            if (backgroundColor != null)
            {
                var colorEnumValue = EnumHelpers.ObjectToEnumValue<DrawingHatchColors>(backgroundColor);
                if (colorEnumValue.HasValue)
                {
                    hatchAttributes.DrawBackgroundColor = true;
                    hatchAttributes.BackgroundColor = colorEnumValue.Value;
                }
            }

            DA.SetData(ParamInfos.HatchAttributes.Name, new ModelObjectHatchAttributesGoo(hatchAttributes));
        }
    }
}
