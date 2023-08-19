using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using static Tekla.Structures.Drawing.ReinforcementBase;

namespace GTDrawingLink.Components
{
    public class ReinforcementAttributesComponent : TeklaComponentBaseNew<ReinforcementAttributesCommand>
    {
        public ReinforcementAttributesComponent() : base(ComponentInfos.ReinforcementAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (ReinforcementSingleAttributes rebarAttributes, string fileName, ReinforcementVisibilityTypes? visibility, StraightEndSymbolTypes? straightEnd, HookedEndSymbolTypes? hookedEnd) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                rebarAttributes.LoadAttributes(fileName);

            if (visibility.HasValue)
                rebarAttributes.ReinforcementVisibility = visibility.Value;

            if (straightEnd.HasValue)
                rebarAttributes.StraightEndSymbolType = straightEnd.Value;

            if (hookedEnd.HasValue)
                rebarAttributes.HookedEndSymbolType = hookedEnd.Value;

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

        private readonly OutputParam<ReinforcementSingleAttributes> _outAttributes = new OutputParam<ReinforcementSingleAttributes>(ParamInfos.RebarAtributes);

        internal (ReinforcementSingleAttributes Attributes,
            string fileName,
            ReinforcementVisibilityTypes? visibility,
            StraightEndSymbolTypes? straightSymbol,
            HookedEndSymbolTypes? hookedSymbol)
            GetInputValues()
        {
            return (
                _inMeshAttributes.Value ?? new ReinforcementSingleAttributes(),
                _inAttributesFileName.Value,
                _inVisibility.ValueProvidedByUser ? _inVisibility.Value : new ReinforcementVisibilityTypes?(),
                _inStraightSymbol.ValueProvidedByUser ? _inStraightSymbol.Value : new StraightEndSymbolTypes?(),
                _inHookedSymbol.ValueProvidedByUser ? _inHookedSymbol.Value : new HookedEndSymbolTypes?()
                );
        }

        internal Result SetOutputValues(IGH_DataAccess DA, ReinforcementSingleAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
