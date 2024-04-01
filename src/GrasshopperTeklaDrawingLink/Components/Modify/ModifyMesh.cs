using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    public class ModifyMeshComponent : TeklaComponentBaseNew<ModifyMeshCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyMesh;
        public ModifyMeshComponent() : base(ComponentInfos.ModifyMeshComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (reinforcements, attributes, customLongitudinal, customCross) = _command.GetInputValues();

            for (int i = 0; i < reinforcements.Count; i++)
            {
                var longitudinal = customLongitudinal.HasItems() ? customLongitudinal.ElementAtOrLast(i).Value : -1;
                var cross = customCross.HasItems() ? customCross.ElementAtOrLast(i).Value : -1;
                ApplyAttributes(reinforcements[i], attributes.ElementAtOrLast(i), longitudinal, cross);
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, reinforcements);
        }

        private void ApplyAttributes(ReinforcementMesh reinforcementBase, ReinforcementBase.ReinforcementMeshAttributes attributes, double longitudinalPosition, double crossPosition)
        {
            reinforcementBase.Attributes = attributes;
            if (RebarCustomPositionChecker.IsValid(longitudinalPosition))
            {
                reinforcementBase.Attributes.MeshReinforcementVisibilityLongitudinal = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                reinforcementBase.ReinforcementCustomPositionLongitudinal = longitudinalPosition;
            }
            if (RebarCustomPositionChecker.IsValid(crossPosition))
            {
                reinforcementBase.Attributes.MeshReinforcementVisibilityCrossing = ReinforcementBase.ReinforcementVisibilityTypes.Customized;
                reinforcementBase.ReinforcementCustomPositionCrossing = crossPosition;
            }

            reinforcementBase.Modify();
        }
    }

    public class ModifyMeshCommand : CommandBase
    {
        private readonly InputListParam<ReinforcementMesh> _inReinforcements = new InputListParam<ReinforcementMesh>(ParamInfos.Mesh);
        private readonly InputListParam<ReinforcementBase.ReinforcementMeshAttributes> _inAttributes = new InputListParam<ReinforcementBase.ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);
        private readonly InputOptionalListParam<GH_Number> _inCustomLongitudinal = new InputOptionalListParam<GH_Number>(ParamInfos.RebarCustomPositionLongitudinal);
        private readonly InputOptionalListParam<GH_Number> _inCustomCross = new InputOptionalListParam<GH_Number>(ParamInfos.RebarCustomPositionCross);

        private readonly OutputListParam<ReinforcementMesh> _outReinforcements = new OutputListParam<ReinforcementMesh>(ParamInfos.Mesh);

        internal (List<ReinforcementMesh> reinforcements, List<ReinforcementBase.ReinforcementMeshAttributes> attributes, List<GH_Number>? customLongitudinal, List<GH_Number>? customCross) GetInputValues()
        {
            return (_inReinforcements.Value, _inAttributes.Value, _inCustomLongitudinal.GetValueFromUserOrNull(), _inCustomCross.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<ReinforcementMesh> reinforcements)
        {
            _outReinforcements.Value = reinforcements;

            return SetOutput(DA);
        }
    }
}
