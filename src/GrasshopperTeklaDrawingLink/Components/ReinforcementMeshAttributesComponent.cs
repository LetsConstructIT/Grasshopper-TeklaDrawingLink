using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;
using static Tekla.Structures.Drawing.ReinforcementBase;

namespace GTDrawingLink.Components
{
    public class ReinforcementMeshAttributesComponent : TeklaComponentBaseNew<ReinforcementMeshAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.MeshAttributes;
        public ReinforcementMeshAttributesComponent() : base(ComponentInfos.ReinforcementMeshAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (ReinforcementMeshAttributes meshAttributes, string fileName, ReinforcementVisibilityTypes? visibilityCross, ReinforcementVisibilityTypes? visibilityLongitudinal, int? symbolIndex, double? symbolSize, ReinforcementRepresentationTypes? representation, LineTypeAttributes? visibileLines, LineTypeAttributes? hiddenLines, bool? hiddenByPart, bool? hiddenByRebars) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                meshAttributes.LoadAttributes(fileName);

            if (visibilityCross.HasValue)
                meshAttributes.MeshReinforcementVisibilityCrossing = visibilityCross.Value;

            if (visibilityLongitudinal.HasValue)
                meshAttributes.MeshReinforcementVisibilityLongitudinal = visibilityLongitudinal.Value;

            if (symbolIndex.HasValue)
                meshAttributes.MeshReinforcementSymbolIndex = symbolIndex.Value;

            if (symbolSize.HasValue)
                meshAttributes.MeshReinforcementSymbolSize = symbolSize.Value;

            if (representation.HasValue)
                meshAttributes.ReinforcementRepresentation = representation.Value;

            if (visibileLines != null)
                meshAttributes.VisibleLines = visibileLines;

            if (hiddenLines != null)
                meshAttributes.HiddenLines = hiddenLines;

            if (hiddenByPart.HasValue)
                meshAttributes.HideLinesHiddenByPart = hiddenByPart.Value;

            if (hiddenByRebars.HasValue)
                meshAttributes.HideLinesHiddenByReinforcement = hiddenByRebars.Value;

            _command.SetOutputValues(DA, meshAttributes);
        }
    }
    public class ReinforcementMeshAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<ReinforcementMeshAttributes> _inMeshAttributes = new InputOptionalParam<ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalStructParam<ReinforcementVisibilityTypes> _inVisibilityCross = new InputOptionalStructParam<ReinforcementVisibilityTypes>(ParamInfos.MeshVisibilityCross);
        private readonly InputOptionalStructParam<ReinforcementVisibilityTypes> _inVisibilityLongitudinal = new InputOptionalStructParam<ReinforcementVisibilityTypes>(ParamInfos.MeshVisibilityLongitudinal);
        private readonly InputOptionalStructParam<int> _inSymbolIndex = new InputOptionalStructParam<int>(ParamInfos.MeshReinforcementSymbolIndex);
        private readonly InputOptionalStructParam<double> _inSymbolSize = new InputOptionalStructParam<double>(ParamInfos.MeshReinforcementSymbolSize);

        private readonly InputOptionalStructParam<ReinforcementRepresentationTypes> _inRepresentation = new InputOptionalStructParam<ReinforcementRepresentationTypes>(ParamInfos.ReinforcementRepresentationTypes);
        private readonly InputOptionalParam<LineTypeAttributes> _inVisibleLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.VisibileLineTypeAttributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inHiddenLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);
        private readonly InputOptionalStructParam<bool> _inHiddenByPart = new InputOptionalStructParam<bool>(ParamInfos.HideLinesHiddenByPart);
        private readonly InputOptionalStructParam<bool> _inHiddenByRebars = new InputOptionalStructParam<bool>(ParamInfos.HideLinesHiddenByReinforcement);

        private readonly OutputParam<ReinforcementMeshAttributes> _outAttributes = new OutputParam<ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);

        internal (ReinforcementMeshAttributes Attributes,
            string fileName,
            ReinforcementVisibilityTypes? visibilityCross,
            ReinforcementVisibilityTypes? visibilityLongitudinal,
            int? symbolIndex,
            double? symbolSize,
            ReinforcementRepresentationTypes? representation,
            LineTypeAttributes? visibileLines,
            LineTypeAttributes? hiddenLines,
            bool? hiddenByPart,
            bool? hiddenByRebars)
            GetInputValues()
        {
            return (
                _inMeshAttributes.Value ?? new ReinforcementMeshAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inVisibilityCross.GetValueFromUserOrNull(),
                _inVisibilityLongitudinal.GetValueFromUserOrNull(),
                _inSymbolIndex.GetValueFromUserOrNull(),
                _inSymbolSize.GetValueFromUserOrNull(),
                _inRepresentation.GetValueFromUserOrNull(),
                _inVisibleLines.GetValueFromUserOrNull(),
                _inHiddenLines.GetValueFromUserOrNull(),
                _inHiddenByPart.GetValueFromUserOrNull(),
                _inHiddenByRebars.GetValueFromUserOrNull()
                );
        }

        internal Result SetOutputValues(IGH_DataAccess DA, ReinforcementMeshAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
