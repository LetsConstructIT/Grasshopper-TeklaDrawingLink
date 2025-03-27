using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            (var inputViews, var basePoints, var insertionPoints, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
            if (!mainInputIsCorrect)
            {
                HandleMissingInput();
                return null;
            }

            if (!DrawingInteractor.IsInTheActiveDrawing(inputViews.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var views = new ViewCollection<View>(inputViews);
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

        private readonly InputOptionalListParam<View> _inView = new InputOptionalListParam<View>(ParamInfos.View);

        private readonly InputTreePoint _inBasePoint = new InputTreePoint(ParamInfos.LevelMarkBasePoint, isOptional: true);
        private readonly InputTreePoint _inInsertionPoint = new InputTreePoint(ParamInfos.LevelMarkInsertionPoint, isOptional: true);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.LevelMarkAttributes, isOptional: true);

        private readonly OutputTreeParam<DatabaseObject> _outMark = new OutputTreeParam<DatabaseObject>(ParamInfos.Mark, 0);

        internal (List<View> views, TreeData<Point3d> basePoint, TreeData<Point3d> insertionPoint, TreeData<string> attributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                    _inBasePoint.AsTreeData(),
                    _inInsertionPoint.AsTreeData(),
                    _inAttributes.IsEmpty() ? _inAttributes.GetDefault(_defaultAttributes) : _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
