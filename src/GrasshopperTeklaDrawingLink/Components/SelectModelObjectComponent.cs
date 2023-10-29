using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class SelectModelObjectComponent : TeklaComponentBaseNew<SelectModelObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.SelectModelObject;

        public SelectModelObjectComponent() : base(ComponentInfos.SelectModelObjectComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var modelObjects = _command.GetInputValues();
            if (modelObjects.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input object could not be casted to Tekla Model Object");
                return;
            }

            ModelInteractor.SelectModelObjects(modelObjects);

            _command.SetOutputValue(DA, true);
        }
    }

    public class SelectModelObjectCommand : CommandBase
    {
        private readonly InputListParam<object> _inModelObject = new InputListParam<object>(ParamInfos.ModelObject);

        private readonly OutputParam<bool> _outStatus = new OutputParam<bool>(ParamInfos.SelectionResult);


        internal List<TSM.ModelObject> GetInputValues()
        {
            return GetModelObjectFromInput(_inModelObject.Value);
        }

        internal Result SetOutputValue(IGH_DataAccess DA, bool status)
        {
            _outStatus.Value = status;

            return SetOutput(DA);
        }

        private List<TSM.ModelObject> GetModelObjectFromInput(List<object> inputObjects)
        {
            var modelObjects = new List<TSM.ModelObject>();
            foreach (var inputObject in inputObjects)
            {
                if (inputObject is GH_Goo<TSM.ModelObject> modelGoo)
                {
                    modelObjects.Add(modelGoo.Value);
                }
            }

            return modelObjects;
        }
    }
}
