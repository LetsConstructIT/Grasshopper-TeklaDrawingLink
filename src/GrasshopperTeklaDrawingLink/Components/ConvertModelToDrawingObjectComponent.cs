using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class ConvertModelToDrawingObjectComponent : TeklaComponentBaseNew<ConvertModelToDrawingObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ConvertModelToDrawingObject;

        public ConvertModelToDrawingObjectComponent() : base(ComponentInfos.ConvertModelToDrawingObjectComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (view, modelObjects) = _command.GetInputValues();

            var drawingObjects = new List<DrawingObject>();
            foreach (var modelObject in modelObjects)
            {
                var doe = view.GetModelObjects(modelObject.Identifier);
                doe.MoveNext();
                drawingObjects.Add(doe.Current);
            }

            _command.SetOutputValues(DA, drawingObjects);
        } 
    }

    public class ConvertModelToDrawingObjectCommand : CommandBase
    {
        private readonly InputParam<ViewBase> _inViews = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputListParam<TSM.ModelObject> _inModelObjects = new InputListParam<TSM.ModelObject>(ParamInfos.ModelObject);

        private readonly OutputListParam<DrawingObject> _outObjects = new OutputListParam<DrawingObject>(ParamInfos.DrawingObject);

        internal (ViewBase view, List<TSM.ModelObject> modelObjects) GetInputValues()
        {
            return (_inViews.Value, _inModelObjects.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<DrawingObject> marks)
        {
            _outObjects.Value = marks;

            return SetOutput(DA);
        }
    }
}
