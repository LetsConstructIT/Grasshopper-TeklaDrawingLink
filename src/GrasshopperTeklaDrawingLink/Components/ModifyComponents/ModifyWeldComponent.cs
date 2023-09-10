using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.ModifyComponents
{
    public class ModifyWeldComponent : TeklaComponentBaseNew<ModifyWeldCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyWeld;
        public ModifyWeldComponent() : base(ComponentInfos.ModifyWeldComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<Weld> welds, List<Weld.WeldAttributes> attributes) = _command.GetInputValues();

            for (int i = 0; i < welds.Count; i++)
            {
                ApplyAttributes(welds[i], attributes.ElementAtOrLast(i));
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, welds);
        }

        private void ApplyAttributes(Weld bolt, Weld.WeldAttributes attributes)
        {
            bolt.Attributes = attributes;
            bolt.Modify();
        }
    }

    public class ModifyWeldCommand : CommandBase
    {
        private readonly InputListParam<Weld> _inWelds = new InputListParam<Weld>(ParamInfos.Weld);
        private readonly InputListParam<Weld.WeldAttributes> _inAttributes = new InputListParam<Weld.WeldAttributes>(ParamInfos.WeldAttributes);

        private readonly OutputListParam<Weld> _outWelds = new OutputListParam<Weld>(ParamInfos.Weld);

        internal (List<Weld> bolts, List<Weld.WeldAttributes> attributes) GetInputValues()
        {
            return (_inWelds.Value, _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Weld> bolts)
        {
            _outWelds.Value = bolts;

            return SetOutput(DA);
        }
    }
}
