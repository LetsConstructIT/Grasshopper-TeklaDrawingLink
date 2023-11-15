using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            var childObjectsGroupedByType = GetRelatedObjectsGroupedByType(drawingObject);


            _command.SetOutputValues(DA, childObjectsGroupedByType.Select(c => c.Key).ToList());
        }
        private IEnumerable<IGrouping<string, DrawingObject>> GetRelatedObjectsGroupedByType(DrawingObject drawingObject)
        {
            var childObjects = new List<DrawingObject>();
            var doe = drawingObject.GetRelatedObjects();
            while (doe.MoveNext())
                childObjects.Add(doe.Current);

            return childObjects.GroupBy(o => o.GetType().ToShortString()).OrderBy(o => o.Key);
        }
    }

    public class GetRelatedObjectsCommand : CommandBase
    {
        private readonly InputParam<DrawingObject> _inDrawingObject = new InputParam<DrawingObject>(ParamInfos.DrawingObject);

        private readonly OutputListParam<string> _outTypes = new OutputListParam<string>(ParamInfos.GroupingKeys);

        internal DrawingObject GetInputValues()
        {
            return _inDrawingObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<string> types)
        {
            _outTypes.Value = types;

            return SetOutput(DA);
        }
    }
}
