using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Tekla.Structures.Drawing;

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
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item);
            pManager.AddParameter(new EnumParam<DimensionSetBaseAttributes.DimensionTypes>(ParamInfos.DimensionLineType, GH_ParamAccess.item, 0));

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var attributesFileName = string.Empty;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFileName);

            var attributes = new StraightDimensionSet.StraightDimensionSetAttributes(attributesFileName);
            SetDimensionType(DA, attributes);

            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(attributes));
        }

        private void SetDimensionType(IGH_DataAccess DA, StraightDimensionSet.StraightDimensionSetAttributes attributes)
        {
            object dimensionTypeInput = null;
            DA.GetData(ParamInfos.DimensionLineType.Name, ref dimensionTypeInput);
            if (dimensionTypeInput != null)
            {
                var dimensionType = EnumHelpers.ObjectToEnumValue<DimensionSetBaseAttributes.DimensionTypes>(dimensionTypeInput).Value;
                attributes.DimensionType = dimensionType;
            }
        }
    }
}
