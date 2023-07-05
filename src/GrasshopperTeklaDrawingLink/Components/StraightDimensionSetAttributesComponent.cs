using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using Tekla.Structures.Drawing;
using static Tekla.Structures.Drawing.StraightDimensionSet;

namespace GTDrawingLink.Components
{
    public class StraightDimensionSetAttributesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public StraightDimensionSetAttributesComponent() : base(ComponentInfos.StraightDimensionSetAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
            pManager.AddTextParameter(ParamInfos.Attributes.Name, ParamInfos.Attributes.NickName, ParamInfos.Attributes.Description, GH_ParamAccess.item);
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.DimensionTypes>(ParamInfos.DimensionLineType, GH_ParamAccess.item));
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.Placings>(ParamInfos.DimensionLinePlacingType, GH_ParamAccess.item));

            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var attributes = DA.GetGooValue<StraightDimensionSetAttributes>(ParamInfos.StraightDimensionSetAttributes);

            var attributesFileName = string.Empty;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFileName);

            if (attributes == null)
            {
                attributes = new StraightDimensionSetAttributes(modelObject: null, attributesFileName);
            }
            else
            {
                if (!string.IsNullOrEmpty(attributesFileName))
                    attributes.LoadAttributes(attributesFileName);
            }

            SetDimensionType(DA, attributes);
            SetPlacingType(DA, attributes);

            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(attributes));
        }

        private void SetDimensionType(IGH_DataAccess DA, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            object dimensionTypeInput = null;
            DA.GetData(ParamInfos.DimensionLineType.Name, ref dimensionTypeInput);
            if (dimensionTypeInput == null)
                return;

            var dimensionType = EnumHelpers.ObjectToEnumValue<DimensionSetBaseAttributes.DimensionTypes>(dimensionTypeInput).Value;
            if (Enum.IsDefined(typeof(DimensionSetBaseAttributes.DimensionTypes), dimensionType))
                attributes.DimensionType = dimensionType;
        }

        private void SetPlacingType(IGH_DataAccess DA, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            object placingTypeInput = null;
            DA.GetData(ParamInfos.DimensionLinePlacingType.Name, ref placingTypeInput);
            if (placingTypeInput == null)
                return;

            var placingType = EnumHelpers.ObjectToEnumValue<DimensionSetBaseAttributes.Placings>(placingTypeInput).Value;
            if (Enum.IsDefined(typeof(DimensionSetBaseAttributes.Placings), placingType))
                attributes.Placing.Placing = placingType;
        }
    }
}
