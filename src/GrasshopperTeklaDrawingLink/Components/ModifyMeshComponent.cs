using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class ModifyMeshComponent : TeklaComponentBaseNew<ModifyMeshCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyMesh;
        public ModifyMeshComponent() : base(ComponentInfos.ModifyMeshComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<ReinforcementMesh> reinforcements, List<ReinforcementBase.ReinforcementMeshAttributes> attributes) = _command.GetInputValues();

            for (int i = 0; i < reinforcements.Count; i++)
            {
                ApplyAttributes(reinforcements[i], attributes.ElementAtOrLast(i));
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, reinforcements);
        }

        private void ApplyAttributes(ReinforcementMesh reinforcementBase, ReinforcementBase.ReinforcementMeshAttributes attributes)
        {
            reinforcementBase.Attributes = attributes;

            reinforcementBase.Modify();
        }
    }

    public class ModifyMeshCommand : CommandBase
    {
        private readonly InputListParam<ReinforcementMesh> _inReinforcements = new InputListParam<ReinforcementMesh>(ParamInfos.Mesh);
        private readonly InputListParam<ReinforcementBase.ReinforcementMeshAttributes> _inAttributes = new InputListParam<ReinforcementBase.ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);


        private readonly OutputListParam<ReinforcementMesh> _outReinforcements = new OutputListParam<ReinforcementMesh>(ParamInfos.Mesh);

        internal (List<ReinforcementMesh> reinforcements, List<ReinforcementBase.ReinforcementMeshAttributes> attributes) GetInputValues()
        {
            return (_inReinforcements.Value, _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<ReinforcementMesh> reinforcements)
        {
            _outReinforcements.Value = reinforcements;

            return SetOutput(DA);
        }
    }
}
