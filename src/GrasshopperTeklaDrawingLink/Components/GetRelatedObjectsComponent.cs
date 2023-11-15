using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Documents;
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

            var relatedObjectsGroupedByType = GetRelatedObjectsGroupedByType(drawingObject);
            var tree = GetOutputTree(relatedObjectsGroupedByType);

            _command.SetOutputValues(DA, tree, relatedObjectsGroupedByType.Select(c => c.Key).ToList());
        }

        private IEnumerable<IGrouping<string, DrawingObject>> GetRelatedObjectsGroupedByType(DrawingObject drawingObject)
        {
            var childObjects = new List<DrawingObject>();
            var doe = drawingObject.GetRelatedObjects();
            while (doe.MoveNext())
                childObjects.Add(doe.Current);

            return childObjects.GroupBy(o => o.GetType().ToShortString()).OrderBy(o => o.Key);
        }

        private IGH_Structure GetOutputTree(IEnumerable<IGrouping<string, DrawingObject>> childObjectsGroupedByType)
        {
            var output = new GH_Structure<TeklaDatabaseObjectGoo>();

            var index = 0;
            foreach (var currentObjects in childObjectsGroupedByType)
            {
                var indicies = currentObjects.Select(o => new TeklaDatabaseObjectGoo(o));
                output.AppendRange(indicies, new GH_Path(0, index));

                index++;
            }

            return output;
        }
    }

    public class GetRelatedObjectsCommand : CommandBase
    {
        private readonly InputParam<DrawingObject> _inDrawingObject = new InputParam<DrawingObject>(ParamInfos.DrawingObject);

        private readonly OutputTreeParam<DatabaseObject> _outObjects = new OutputTreeParam<DatabaseObject>(ParamInfos.TeklaDatabaseObject, 0);
        private readonly OutputListParam<string> _outTypes = new OutputListParam<string>(ParamInfos.GroupingKeys);

        internal DrawingObject GetInputValues()
        {
            return _inDrawingObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure objects, List<string> types)
        {
            _outObjects.Value = objects;
            _outTypes.Value = types;

            return SetOutput(DA);
        }
    }
}
