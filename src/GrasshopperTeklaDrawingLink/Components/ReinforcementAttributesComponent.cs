using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;
using static Tekla.Structures.Drawing.ReinforcementBase;

namespace GTDrawingLink.Components
{
    public class ReinforcementAttributesComponent : TeklaComponentBaseNew<ReinforcementAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.RebarAttributes;
        public ReinforcementAttributesComponent() : base(ComponentInfos.ReinforcementAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (ReinforcementSingleAttributes rebarAttributes, string fileName, ReinforcementVisibilityTypes? visibility, StraightEndSymbolTypes? straightSymbol, HookedEndSymbolTypes? hookedSymbol, ReinforcementRepresentationTypes? representation, LineTypeAttributes? visibileLines, LineTypeAttributes? hiddenLines, bool? hiddenByPart, bool? hiddenByRebars) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                rebarAttributes.LoadAttributes(fileName);

            if (visibility.HasValue)
                rebarAttributes.ReinforcementVisibility = visibility.Value;

            if (straightSymbol.HasValue)
                rebarAttributes.StraightEndSymbolType = straightSymbol.Value;

            if (hookedSymbol.HasValue)
                rebarAttributes.HookedEndSymbolType = hookedSymbol.Value;

            if (representation.HasValue)
                rebarAttributes.ReinforcementRepresentation = representation.Value;

            if (visibileLines != null)
                rebarAttributes.VisibleLines = visibileLines;

            if (hiddenLines != null)
                rebarAttributes.HiddenLines = hiddenLines;

            if (hiddenByPart.HasValue)
                rebarAttributes.HideLinesHiddenByPart = hiddenByPart.Value;

            if (hiddenByRebars.HasValue)
                rebarAttributes.HideLinesHiddenByReinforcement = hiddenByRebars.Value;

            _command.SetOutputValues(DA, rebarAttributes);
        }
    }

    public class ReinforcementAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<ReinforcementSingleAttributes> _inMeshAttributes = new InputOptionalParam<ReinforcementSingleAttributes>(ParamInfos.RebarAtributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalStructParam<ReinforcementVisibilityTypes> _inVisibility = new InputOptionalStructParam<ReinforcementVisibilityTypes>(ParamInfos.RebarVisibility);
        private readonly InputOptionalStructParam<StraightEndSymbolTypes> _inStraightSymbol = new InputOptionalStructParam<StraightEndSymbolTypes>(ParamInfos.StraightEndSymbolTypes);
        private readonly InputOptionalStructParam<HookedEndSymbolTypes> _inHookedSymbol = new InputOptionalStructParam<HookedEndSymbolTypes>(ParamInfos.HookedEndSymbolTypes);

        private readonly InputOptionalStructParam<ReinforcementRepresentationTypes> _inRepresentation = new InputOptionalStructParam<ReinforcementRepresentationTypes>(ParamInfos.ReinforcementRepresentationTypes);
        private readonly InputOptionalParam<LineTypeAttributes> _inVisibleLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.VisibileLineTypeAttributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inHiddenLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);
        private readonly InputOptionalStructParam<bool> _inHiddenByPart = new InputOptionalStructParam<bool>(ParamInfos.HideLinesHiddenByPart);
        private readonly InputOptionalStructParam<bool> _inHiddenByRebars = new InputOptionalStructParam<bool>(ParamInfos.HideLinesHiddenByReinforcement);

        private readonly OutputParam<ReinforcementSingleAttributes> _outAttributes = new OutputParam<ReinforcementSingleAttributes>(ParamInfos.RebarAtributes);

        internal (ReinforcementSingleAttributes Attributes,
            string fileName,
            ReinforcementVisibilityTypes? visibility,
            StraightEndSymbolTypes? straightSymbol,
            HookedEndSymbolTypes? hookedSymbol,
            ReinforcementRepresentationTypes? representation,
            LineTypeAttributes? visibileLines,
            LineTypeAttributes? hiddenLines,
            bool? hiddenByPart,
            bool? hiddenByRebars)
            GetInputValues()
        {
            return (
                _inMeshAttributes.Value ?? new ReinforcementSingleAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inVisibility.GetValueFromUserOrNull(),
                _inStraightSymbol.GetValueFromUserOrNull(),
                _inHookedSymbol.GetValueFromUserOrNull(),
                _inRepresentation.GetValueFromUserOrNull(),
                _inVisibleLines.GetValueFromUserOrNull(),
                _inHiddenLines.GetValueFromUserOrNull(),
                _inHiddenByPart.GetValueFromUserOrNull(),
                _inHiddenByRebars.GetValueFromUserOrNull()
                );
        }

        internal Result SetOutputValues(IGH_DataAccess DA, ReinforcementSingleAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
