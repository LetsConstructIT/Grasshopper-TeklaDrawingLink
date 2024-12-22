using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components.Miscs
{
    public class GetSelectedModelObjectComponent : TeklaComponentBaseNew<GetSelectedModelObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.SelectededModelObjects;

        public GetSelectedModelObjectComponent() : base(ComponentInfos.GetSelectedModelObjectComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var trigger = _command.GetInputValues();
            if (!trigger)
                return;

            var selectedObjects = new List<TSM.ModelObject>();

            var moe = new TSM.UI.ModelObjectSelector().GetSelectedObjects();
            while (moe.MoveNext())
                selectedObjects.Add(moe.Current);

            _command.SetOutputValue(DA, selectedObjects);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }

    public class GetSelectedModelObjectCommand : CommandBase
    {
        private readonly InputStructParam<bool> _inTrigger = new InputStructParam<bool>(ParamInfos.BooleanToggle);

        private readonly OutputListParam<TSM.ModelObject> _outSelectedObjects = new OutputListParam<TSM.ModelObject>(ParamInfos.ModelObject);


        internal bool GetInputValues()
        {
            return _inTrigger.Value;
        }

        internal Result SetOutputValue(IGH_DataAccess DA, List<TSM.ModelObject> objects)
        {
            _outSelectedObjects.Value = objects;

            return SetOutput(DA);
        }
    }
}
