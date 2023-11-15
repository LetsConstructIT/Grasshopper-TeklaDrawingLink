using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetRelatedObjectsComponent : TeklaComponentBaseNew<GetRelatedObjectsCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.GetRelatedObjects;

        public GetRelatedObjectsComponent() : base(ComponentInfos.GetRelatedObjectsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawingObject = _command.GetInputValues();

            //var result = DrawingInteractor.DrawingHandler.CloseActiveDrawing(save);

            _command.SetOutputValues(DA, true);
        }
    }

    public class GetRelatedObjectsCommand : CommandBase
    {
        private readonly InputParam<DrawingObject> _inDrawingObject = new InputParam<DrawingObject>(ParamInfos.DrawingObject);

        private readonly OutputParam<bool> _outResult = new OutputParam<bool>(ParamInfos.DrawingSaveResult);

        internal DrawingObject GetInputValues()
        {
            return _inDrawingObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, bool result)
        {
            _outResult.Value = result;

            return SetOutput(DA);
        }
    }
}
