using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class SelectModelObjectComponent : TeklaComponentBaseNew<SelectModelObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.SelectModelObject;

        public SelectModelObjectComponent() : base(ComponentInfos.SelectModelObjectComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var input = _command.GetInputValues();
            if (input.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input object could not be casted to Tekla Model Object");
                return;
            }

            var modelObjects = new List<TSM.ModelObject>();
            foreach (var item in input)
                modelObjects.AddRange(item);

            ModelInteractor.SelectModelObjects(modelObjects);

            _command.SetOutputValue(DA, true);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }

    public class SelectModelObjectCommand : CommandBase
    {
        private readonly InputTreeParam<TSM.ModelObject> _inModelObjects = new InputTreeParam<TSM.ModelObject>(ParamInfos.ModelObject);

        private readonly OutputParam<bool> _outStatus = new OutputParam<bool>(ParamInfos.SelectionResult);


        internal List<List<TSM.ModelObject>> GetInputValues()
        {
            return _inModelObjects.Value;
        }

        internal Result SetOutputValue(IGH_DataAccess DA, bool status)
        {
            _outStatus.Value = status;

            return SetOutput(DA);
        }
    }
}
