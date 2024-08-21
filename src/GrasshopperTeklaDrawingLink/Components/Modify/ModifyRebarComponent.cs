using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
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
            (List<ReinforcementBase> reinforcements, List<ReinforcementBase.ReinforcementSingleAttributes> attributes, List<GH_Number>? customPosition) = _command.GetInputValues();

            for (int i = 0; i < reinforcements.Count; i++)
            {
                var position = customPosition.HasItems() ? customPosition.ElementAtOrLast(i).Value : -1;
                var attribute = attributes.HasItems() ? attributes.ElementAtOrLast(i) : null;
                ApplyAttributes(reinforcements[i], attribute, position);
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, reinforcements);
        }

        private void ApplyAttributes(ReinforcementBase reinforcementBase, ReinforcementBase.ReinforcementSingleAttributes? attributes, double customPosition)
        {
            if (reinforcementBase is ReinforcementSingle single)
            {
                if (attributes != null)
                    single.Attributes = attributes;

                if (RebarCustomPositionChecker.IsValid(customPosition))
                {
                    single.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    single.ReinforcementCustomPosition = customPosition;
                }
            }
            else if (reinforcementBase is ReinforcementGroup group)
            {
                if (attributes != null)
                    group.Attributes = CastToGroupAttributes(attributes);

                if (RebarCustomPositionChecker.IsValid(customPosition))
                {
                    group.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    group.ReinforcementCustomPosition = customPosition;
                }
            }
            else if (reinforcementBase is ReinforcementSetGroup set)
            {
                if (attributes != null)
                    set.Attributes = CastToSetAttributes(attributes);

                if (RebarCustomPositionChecker.IsValid(customPosition))
                {
                    set.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    set.ReinforcementCustomPosition = customPosition;
                }
            }
            else if (reinforcementBase is ReinforcementStrand strand)
            {
                if (attributes != null)
                    strand.Attributes = CastToStrandAttributes(attributes);

                if (RebarCustomPositionChecker.IsValid(customPosition))
                {
                    strand.Attributes.ReinforcementVisibility = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                    strand.ReinforcementCustomPosition = customPosition;
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
        private readonly InputListParam<ReinforcementBase> _inReinforcements = new InputListParam<ReinforcementBase>(ParamInfos.Reinforcement);
        private readonly InputOptionalListParam<ReinforcementBase.ReinforcementSingleAttributes> _inAttributes = new InputOptionalListParam<ReinforcementBase.ReinforcementSingleAttributes>(ParamInfos.RebarAtributes);
        private readonly InputOptionalListParam<GH_Number> _inCustomPosition = new InputOptionalListParam<GH_Number>(ParamInfos.RebarCustomPosition);

        private readonly OutputListParam<ReinforcementBase> _outReinforcements = new OutputListParam<ReinforcementBase>(ParamInfos.Reinforcement);

        internal (List<ReinforcementBase> reinforcements, List<ReinforcementBase.ReinforcementSingleAttributes>? attributes, List<GH_Number>? customPosition) GetInputValues()
        {
            return (_inReinforcements.Value, _inAttributes.GetValueFromUserOrNull(), _inCustomPosition.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<ReinforcementBase> reinforcements)
        {
            _outReinforcements.Value = reinforcements;

            return SetOutput(DA);
        }
    }
}
