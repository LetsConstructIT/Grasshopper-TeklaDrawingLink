using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;
using static Tekla.Structures.Drawing.StraightDimensionSet;

namespace GTDrawingLink.Components.Obsolete
{
    public class StraightDimensionSetAttributesComponentOLD : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.DimensionLineAttributes;

        public StraightDimensionSetAttributesComponentOLD() : base(ComponentInfos.StraightDimensionSetAttributesComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
            pManager.AddTextParameter(ParamInfos.Attributes.Name, ParamInfos.Attributes.NickName, ParamInfos.Attributes.Description, GH_ParamAccess.item);
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.DimensionTypes>(ParamInfos.DimensionLineType, GH_ParamAccess.item));
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.Placings>(ParamInfos.DimensionLinePlacingType, GH_ParamAccess.item));
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.ShortDimensionTypes>(ParamInfos.ShortDimensionType, GH_ParamAccess.item));
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.ExtensionLineTypes>(ParamInfos.ExtensionLineType, GH_ParamAccess.item));
            pManager.AddTextParameter(ParamInfos.ExcludePartsAccordingToFilter.Name, ParamInfos.ExcludePartsAccordingToFilter.NickName, ParamInfos.ExcludePartsAccordingToFilter.Description, GH_ParamAccess.item);

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
                if (string.IsNullOrEmpty(attributesFileName))
                    attributesFileName = "standard";

                attributes = new StraightDimensionSetAttributes(modelObject: null, attributesFileName);
            }
            else
            {
                if (!string.IsNullOrEmpty(attributesFileName))
                    attributes.LoadAttributes(attributesFileName);
            }

            SetDimensionType(DA, attributes);
            SetPlacingType(DA, attributes);
            SetShortDimensionType(DA, attributes);
            SetExtensionLineType(DA, attributes);
            SetExcludeFilter(DA, attributes);

            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(attributes));
        }

        private void SetDimensionType(IGH_DataAccess DA, StraightDimensionSetAttributes attributes)
        {
            var enumValue = DA.GetEnum<DimensionSetBaseAttributes.DimensionTypes>(ParamInfos.DimensionLineType);
            if (enumValue.HasValue)
                attributes.DimensionType = enumValue.Value;
        }

        private void SetPlacingType(IGH_DataAccess DA, DimensionSetBaseAttributes attributes)
        {
            var enumValue = DA.GetEnum<DimensionSetBaseAttributes.Placings>(ParamInfos.DimensionLinePlacingType);
            if (enumValue.HasValue)
                attributes.Placing.Placing = enumValue.Value;
        }

        private void SetShortDimensionType(IGH_DataAccess DA, StraightDimensionSetAttributes attributes)
        {
            var enumValue = DA.GetEnum<DimensionSetBaseAttributes.ShortDimensionTypes>(ParamInfos.ShortDimensionType);
            if (enumValue.HasValue)
                attributes.ShortDimension = enumValue.Value;
        }

        private void SetExtensionLineType(IGH_DataAccess DA, StraightDimensionSetAttributes attributes)
        {
            var enumValue = DA.GetEnum<DimensionSetBaseAttributes.ExtensionLineTypes>(ParamInfos.ExtensionLineType);
            if (enumValue.HasValue)
                attributes.ExtensionLine = enumValue.Value;
        }

        private void SetExcludeFilter(IGH_DataAccess DA, StraightDimensionSetAttributes attributes)
        {
            var excludeFilter = string.Empty;
            if (DA.GetData(ParamInfos.ExcludePartsAccordingToFilter.Name, ref excludeFilter) && !string.IsNullOrEmpty(excludeFilter))
                attributes.ExcludePartsAccordingToFilter = excludeFilter;
        }
    }
}
