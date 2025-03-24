using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Miscs
{
    public class GetProjectPropertiesComponent : TeklaComponentBaseNew<GetProjectPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ProjectProperties;
        public GetProjectPropertiesComponent() : base(ComponentInfos.GetProjectPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var toggle = _command.GetInputValues();
            if (!toggle)
                return;

            var modelInfo = new Model().GetInfo();
            var projectInfo = new Model().GetProjectInfo();

            var fakeProjectObject = new Beam { Identifier = { ID = 7 } };

            _command.SetOutputValues(DA, modelInfo.ModelName, modelInfo.ModelPath, projectInfo, fakeProjectObject);
        }
    }

    public class GetProjectPropertiesCommand : CommandBase
    {
        private readonly InputStructParam<bool> _inToggle = new InputStructParam<bool>(ParamInfos.BooleanToggle);

        private readonly OutputParam<string> _outModelName = new OutputParam<string>(ParamInfos.ModelName);
        private readonly OutputParam<string> _outModelPath = new OutputParam<string>(ParamInfos.ModelPath);
        private readonly OutputParam<ProjectInfo> _outProjectInfo = new OutputParam<ProjectInfo>(ParamInfos.ProjectInfo);
        private readonly OutputParam<ModelObject> _outFakeProjectObject = new OutputParam<ModelObject>(ParamInfos.FakeProjectObject);

        internal bool GetInputValues()
        {
            return _inToggle.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string modelName, string modelPath, ProjectInfo projectInfo, ModelObject fakeProjectObject)
        {
            _outModelName.Value = modelName;
            _outModelPath.Value = modelPath;
            _outProjectInfo.Value = projectInfo;
            _outFakeProjectObject.Value = fakeProjectObject;

            return SetOutput(DA);
        }
    }
}
