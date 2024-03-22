using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateSectionMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateSectionMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.SectionMark;

        public CreateSectionMarkComponent() : base(ComponentInfos.CreateSectionMarkComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var startPoints, var endPoints, var attributes, var names) = _command.GetInputValues();

            var strategy = GetSolverStrategy(startPoints, endPoints, attributes, names);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertSectionMark(views.Get(path),
                                             startPoints.Get(i, inputMode),
                                             endPoints.Get(i, inputMode),
                                             attributes.Get(i, inputMode),
                                             names.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static SectionMark InsertSectionMark(View view, Point3d startPoint, Point3d endPoint, string attributes, string name)
        {
            var sectionAttributes = new SectionMark.SectionMarkAttributes(attributes)
            {
                MarkName = name
            };

            var mark = new SectionMark(view, startPoint.ToTekla(), endPoint.ToTekla(), sectionAttributes);
            mark.Insert();

            return mark;
        }
    }

    public class CreateSectionMarkCommand : CommandBase
    {
        private const string _defaultAttributes = "standard";

        private readonly InputListParam<View> _inView = new InputListParam<View>(ParamInfos.View);

        private readonly InputTreePoint _inStartPoint = new InputTreePoint(ParamInfos.SectionStartPoint);
        private readonly InputTreePoint _inEndPoint = new InputTreePoint(ParamInfos.SectionEndPoint);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.DetailMarkAttributes, isOptional: true);
        private readonly InputTreeString _inName = new InputTreeString(ParamInfos.Name);

        private readonly OutputTreeParam<DatabaseObject> _outMark = new OutputTreeParam<DatabaseObject>(ParamInfos.Mark, 0);

        internal (ViewCollection view, TreeData<Point3d> startPoint, TreeData<Point3d> endPoint, TreeData<string> attributes, TreeData<string> name) GetInputValues()
        {
            return (new ViewCollection(_inView.Value),
                    _inStartPoint.AsTreeData(),
                    _inEndPoint.AsTreeData(),
                    _inAttributes.IsEmpty() ? _inAttributes.GetDefault(_defaultAttributes) : _inAttributes.AsTreeData(),
                    _inName.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure mark)
        {
            _outMark.Value = mark;

            return SetOutput(DA);
        }
    }
}
