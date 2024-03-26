using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateLevelMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateLevelMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.LevelMark;

        public CreateLevelMarkComponent() : base(ComponentInfos.CreateLevelMarkComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var basePoints, var insertionPoints, var attributes) = _command.GetInputValues();

            var strategy = GetSolverStrategy(false, insertionPoints, basePoints, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertLevelMark(views.Get(path),
                                            insertionPoints.Get(i, inputMode),
                                            basePoints.Get(i, inputMode),
                                            attributes.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static LevelMark InsertLevelMark(View view,
                                            Point3d insertionPoint,
                                            Point3d basePoint,
                                            string attribute)
        {
            var levelAttributes = new LevelMark.LevelMarkAttributes(attribute);

            var levelMark = new LevelMark(view, insertionPoint.ToTekla(), basePoint.ToTekla(), levelAttributes);
            levelMark.Insert();

            return levelMark;
        }
    }

    public class CreateLevelMarkCommand : CommandBase
    {
        private const string _defaultAttributes = "standard";

        private readonly InputListParam<View> _inView = new InputListParam<View>(ParamInfos.View);

        private readonly InputTreePoint _inBasePoint = new InputTreePoint(ParamInfos.LevelMarkBasePoint);
        private readonly InputTreePoint _inInsertionPoint = new InputTreePoint(ParamInfos.LevelMarkInsertionPoint);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.LevelMarkAttributes, isOptional: true);

        private readonly OutputTreeParam<DatabaseObject> _outMark = new OutputTreeParam<DatabaseObject>(ParamInfos.Mark, 0);

        internal (ViewCollection<View> views, TreeData<Point3d> basePoint, TreeData<Point3d> insertionPoint, TreeData<string> attributes) GetInputValues()
        {
            return (new ViewCollection<View>(_inView.Value),
                    _inBasePoint.AsTreeData(),
                    _inInsertionPoint.AsTreeData(),
                    _inAttributes.IsEmpty() ? _inAttributes.GetDefault(_defaultAttributes) : _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
