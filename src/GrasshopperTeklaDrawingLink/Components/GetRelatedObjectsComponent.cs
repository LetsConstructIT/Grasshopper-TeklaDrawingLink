using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
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
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetRelatedObjects;

        public GetRelatedObjectsComponent() : base(ComponentInfos.GetRelatedObjectsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (inputTree, paths) = _command.GetInputValues();

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputKeys = new GH_Structure<GH_String>();

            for (int i = 0; i < inputTree.Count; i++)
            {
                var branchObjects = inputTree[i];
                var path = paths[i];

                for (int j = 0; j < branchObjects.Count; j++)
                {
                    var newPath = path.AppendElement(j);

                    var drawingObject = branchObjects[j];

                    var relatedObjectsGroupedByType = GetRelatedObjectsGroupedByType(drawingObject).ToList();
                    if (relatedObjectsGroupedByType.Count == 0)
                        continue;

                    for (int k = 0; k < relatedObjectsGroupedByType.Count; k++)
                    {
                        var objectGoos = relatedObjectsGroupedByType[k].Select(o => new TeklaDatabaseObjectGoo(o));
                        outputTree.AppendRange(objectGoos, newPath.AppendElement(k));
                    }

                    var types = relatedObjectsGroupedByType.Select(c => new GH_String(c.Key)).ToList();
                    outputKeys.AppendRange(types, newPath);
                }
            }

            _command.SetOutputValues(DA, outputTree, outputKeys);
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
        private readonly InputTreeParam<DrawingObject> _inDrawingObject = new InputTreeParam<DrawingObject>(ParamInfos.DrawingObject);

        private readonly OutputTreeParam<DatabaseObject> _outObjects = new OutputTreeParam<DatabaseObject>(ParamInfos.TeklaDatabaseObject, 0);
        private readonly OutputTreeParam<GH_String> _outTypes = new OutputTreeParam<GH_String>(ParamInfos.GroupingKeys, 1);

        internal (List<List<DrawingObject>> objects, IReadOnlyList<GH_Path> paths) GetInputValues()
        {
            return (_inDrawingObject.Value, _inDrawingObject.Paths);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure objects, IGH_Structure types)
        {
            _outObjects.Value = objects;
            _outTypes.Value = types;

            return SetOutput(DA);
        }
    }
}
