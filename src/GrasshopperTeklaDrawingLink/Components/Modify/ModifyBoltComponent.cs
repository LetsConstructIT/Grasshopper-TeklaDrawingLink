using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.ModifyComponents
{
    public class ModifyBoltComponent : TeklaComponentBaseNew<ModifyBoltCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyBolt;
        public ModifyBoltComponent() : base(ComponentInfos.ModifyBoltComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<Bolt> bolts, List<Bolt.BoltAttributes> attributes) = _command.GetInputValues();

            for (int i = 0; i < bolts.Count; i++)
            {
                ApplyAttributes(bolts[i], attributes.ElementAtOrLast(i));
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, bolts);
        }

        private void ApplyAttributes(Bolt bolt, Bolt.BoltAttributes attributes)
        {
            bolt.Attributes = attributes;
            bolt.Modify();
        }
    }

    public class ModifyBoltCommand : CommandBase
    {
        private readonly InputListParam<Bolt> _inBolts = new InputListParam<Bolt>(ParamInfos.Bolt);
        private readonly InputListParam<Bolt.BoltAttributes> _inAttributes = new InputListParam<Bolt.BoltAttributes>(ParamInfos.BoltAttributes);

        private readonly OutputListParam<Bolt> _outReinforcements = new OutputListParam<Bolt>(ParamInfos.Bolt);

        internal (List<Bolt> bolts, List<Bolt.BoltAttributes> attributes) GetInputValues()
        {
            return (_inBolts.Value, _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Bolt> bolts)
        {
            _outReinforcements.Value = bolts;

            return SetOutput(DA);
        }
    }
}
