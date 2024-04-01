using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.ModifyComponents
{
    public class ModifyRebarComponent : TeklaComponentBaseNew<ModifyRebarCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyRebar;
        public ModifyRebarComponent() : base(ComponentInfos.ModifyRebarComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (ReinforcementBase reinforcement, ReinforcementBase.ReinforcementSingleAttributes attributes, double? customPosition) = _command.GetInputValues();

            ApplyAttributes(reinforcement, attributes, customPosition);

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, reinforcement);
        }

        private void ApplyAttributes(ReinforcementBase reinforcementBase, ReinforcementBase.ReinforcementSingleAttributes attributes, double? customPosition)
        {
            if (reinforcementBase is ReinforcementSingle single)
            {
                single.Attributes = attributes;
                if (customPosition.HasValue)
                {
                    single.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    single.ReinforcementCustomPosition = customPosition.Value;
                }
            }
            else if (reinforcementBase is ReinforcementGroup group)
            {
                group.Attributes = CastToGroupAttributes(attributes);
                if (customPosition.HasValue)
                {
                    group.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    group.ReinforcementCustomPosition = customPosition.Value;
                }
            }
            else if (reinforcementBase is ReinforcementSetGroup set)
            {
                set.Attributes = CastToSetAttributes(attributes);
                if (customPosition.HasValue)
                {
                    set.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    set.ReinforcementCustomPosition = customPosition.Value;
                }
            }
            else if (reinforcementBase is ReinforcementStrand strand)
            {
                strand.Attributes = CastToStrandAttributes(attributes);
                if (customPosition.HasValue)
                {
                    strand.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    strand.ReinforcementCustomPosition = customPosition.Value;
                }
            }

            reinforcementBase.Modify();
        }

        private ReinforcementBase.ReinforcementGroupAttributes CastToGroupAttributes(ReinforcementBase.ReinforcementSingleAttributes attributes)
        {
            return new ReinforcementBase.ReinforcementGroupAttributes()
            {
                CustomPresentation = attributes.CustomPresentation,
                HiddenLines = attributes.HiddenLines,
                HideLinesHiddenByPart = attributes.HideLinesHiddenByPart,
                HideLinesHiddenByReinforcement = attributes.HideLinesHiddenByReinforcement,
                HookedEndSymbolType = attributes.HookedEndSymbolType,
                ReinforcementRepresentation = attributes.ReinforcementRepresentation,
                ReinforcementVisibility = attributes.ReinforcementVisibility,
                StraightEndSymbolType = attributes.StraightEndSymbolType,
                VisibleLines = attributes.VisibleLines
            };
        }

        private ReinforcementBase.ReinforcementSetGroupAttributes CastToSetAttributes(ReinforcementBase.ReinforcementSingleAttributes attributes)
        {
            return new ReinforcementBase.ReinforcementSetGroupAttributes()
            {
                CustomPresentation = attributes.CustomPresentation,
                HiddenLines = attributes.HiddenLines,
                HideLinesHiddenByPart = attributes.HideLinesHiddenByPart,
                HideLinesHiddenByReinforcement = attributes.HideLinesHiddenByReinforcement,
                HookedEndSymbolType = attributes.HookedEndSymbolType,
                ReinforcementRepresentation = attributes.ReinforcementRepresentation,
                ReinforcementVisibility = attributes.ReinforcementVisibility,
                StraightEndSymbolType = attributes.StraightEndSymbolType,
                VisibleLines = attributes.VisibleLines
            };
        }

        private ReinforcementBase.ReinforcementStrandAttributes CastToStrandAttributes(ReinforcementBase.ReinforcementSingleAttributes attributes)
        {
            return new ReinforcementBase.ReinforcementStrandAttributes()
            {
                CustomPresentation = attributes.CustomPresentation,
                HiddenLines = attributes.HiddenLines,
                HideLinesHiddenByPart = attributes.HideLinesHiddenByPart,
                HideLinesHiddenByReinforcement = attributes.HideLinesHiddenByReinforcement,
                HookedEndSymbolType = attributes.HookedEndSymbolType,
                ReinforcementRepresentation = attributes.ReinforcementRepresentation,
                ReinforcementVisibility = attributes.ReinforcementVisibility,
                StraightEndSymbolType = attributes.StraightEndSymbolType,
                VisibleLines = attributes.VisibleLines
            };
        }
    }

    public class ModifyRebarCommand : CommandBase
    {
        private readonly InputParam<ReinforcementBase> _inReinforcements = new InputParam<ReinforcementBase>(ParamInfos.Reinforcement);
        private readonly InputParam<ReinforcementBase.ReinforcementSingleAttributes> _inAttributes = new InputParam<ReinforcementBase.ReinforcementSingleAttributes>(ParamInfos.RebarAtributes);
        private readonly InputOptionalStructParam<double> _inCustomPosition = new InputOptionalStructParam<double>(ParamInfos.RebarCustomPosition);

        private readonly OutputParam<ReinforcementBase> _outReinforcements = new OutputParam<ReinforcementBase>(ParamInfos.Reinforcement);

        internal (ReinforcementBase reinforcement, ReinforcementBase.ReinforcementSingleAttributes attributes, double? customPosition) GetInputValues()
        {
            return (_inReinforcements.Value, _inAttributes.Value, _inCustomPosition.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, ReinforcementBase reinforcements)
        {
            _outReinforcements.Value = reinforcements;

            return SetOutput(DA);
        }
    }
}
