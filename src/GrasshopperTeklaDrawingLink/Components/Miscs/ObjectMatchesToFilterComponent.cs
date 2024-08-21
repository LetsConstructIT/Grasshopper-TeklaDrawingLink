using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Linq;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components.Miscs
{
    public class ObjectMatchesToFilterComponent : TeklaComponentBaseNew<ObjectMatchesToFilterCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ObjectMatchesToFilter;
        public ObjectMatchesToFilterComponent() : base(ComponentInfos.ObjectMatchesToFilterComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<TSM.ModelObject> modelObjects, string filterName) = _command.GetInputValues();
            if (modelObjects.Any(o => o is null))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One of the provided model objects does not exist");
                return;
            }

            var filtered = new List<TSM.ModelObject>();
            var pattern = new List<bool>();

            foreach (var modelObject in modelObjects)
            {
                var isMatch = TSM.Operations.Operation.ObjectMatchesToFilter(modelObject, filterName);
                pattern.Add(isMatch);

                if (isMatch)
                    filtered.Add(modelObject);
            }

            _command.SetOutputValues(DA, filtered, pattern);
        }
    }

    public class ObjectMatchesToFilterCommand : CommandBase
    {
        private readonly InputListParam<TSM.ModelObject> _inModelObjects = new InputListParam<TSM.ModelObject>(ParamInfos.ModelObject);
        private readonly InputParam<string> _inFilterName = new InputParam<string>(ParamInfos.ObjectFilter);


        private readonly OutputListParam<TSM.ModelObject> _outModelObjects = new OutputListParam<TSM.ModelObject>(ParamInfos.FilteredModelObjects);
        private readonly OutputListParam<bool> _outPattern = new OutputListParam<bool>(ParamInfos.FilterPattern);

        internal (List<TSM.ModelObject> ModelObjects, string FilterName) GetInputValues()
        {
            return (_inModelObjects.Value,
                    _inFilterName.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSM.ModelObject> filteredObjects, List<bool> pattern)
        {
            _outModelObjects.Value = filteredObjects;
            _outPattern.Value = pattern;

            return SetOutput(DA);
        }
    }
}
