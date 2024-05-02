using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Views
{
    public class CreateCurvedSectionViewComponent : CreateViewBaseComponentNew<CreateCurvedSectionViewCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.CurvedSectionView;

        public CreateCurvedSectionViewComponent() : base(ComponentInfos.CreateCurvedSectionViewComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (views, startPoints, midPoints, endPoints, insertPoints, depthsUp, depthsDown, viewAttributes, markAttributes, scales, names) = _command.GetInputValues();
            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var strategy = GetSolverStrategy(false, startPoints, endPoints, insertPoints, depthsUp, depthsDown, viewAttributes, markAttributes, scales, names);
            var inputMode = strategy.Mode;

            var outputViewTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputMarkTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                (View view, CurvedSectionMark mark) = InsertView(views.Get(path),
                                                           startPoints.Get(i, inputMode),
                                                           midPoints.Get(i, inputMode),
                                                           endPoints.Get(i, inputMode),
                                                           insertPoints.Get(i, inputMode),
                                                           depthsUp.Get(i, inputMode),
                                                           depthsDown.Get(i, inputMode),
                                                           viewAttributes.Get(i, inputMode),
                                                           markAttributes.Get(i, inputMode),
                                                           scales.Get(i, inputMode),
                                                           names.Get(i, inputMode));

                outputObjects.Add(mark);
                outputObjects.Add(view);
                outputViewTree.Append(new TeklaDatabaseObjectGoo(view), path);
                outputMarkTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputViewTree, outputMarkTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private (View view, CurvedSectionMark mark) InsertView(View view,
                                                         Point3d startPoint,
                                                         Point3d midPoint,
                                                         Point3d endPoint,
                                                         Point3d insertionPoint,
                                                         double depthUp,
                                                         double depthDown,
                                                         string viewAttributesFileName,
                                                         string markAttributesFileName,
                                                         double scale,
                                                         string viewName)
        {
            var markAttributes = new SectionMarkBase.SectionMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            var viewAttributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(viewAttributesFileName))
                viewAttributes.LoadAttributes(viewAttributesFileName);

            if (scale > 1)
                viewAttributes.Scale = scale;

            View.CreateCurvedSectionView(
                view,
                startPoint.ToTekla(),
                midPoint.ToTekla(),
                endPoint.ToTekla(),
                insertionPoint.ToTekla(),
                depthUp,
                depthDown,
                viewAttributes,
                markAttributes,
                out View createdView,
                out CurvedSectionMark createdMark);

            var macroApplied = LoadAttributesWithMacroIfNecessary(createdView, viewAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
            {
                createdMark.Attributes.MarkName = viewName;
                createdMark.Modify();

                createdView.Name = viewName;
                createdView.Modify();
            }

            return (createdView, createdMark);
        }
    }

    public class CreateCurvedSectionViewCommand : CommandBase
    {
        private const double _defaultDepth = 500;
        private const string _defaultAttributes = "standard";

        private readonly InputListParam<View> _inView = new InputListParam<View>(ParamInfos.View);

        private readonly InputTreePoint _inStartPoint = new InputTreePoint(ParamInfos.SectionStartPoint);
        private readonly InputTreePoint _inMidPoint = new InputTreePoint(ParamInfos.CurvedSectionMidPoint);
        private readonly InputTreePoint _inEndPoint = new InputTreePoint(ParamInfos.SectionEndPoint);
        private readonly InputTreePoint _inInsertionPoint = new InputTreePoint(ParamInfos.SectionInsertionPoint);

        private readonly InputTreeNumber _inDepthUp = new InputTreeNumber(ParamInfos.SectionDepthUp, isOptional: true);
        private readonly InputTreeNumber _inDepthDown = new InputTreeNumber(ParamInfos.SectionDepthDown, isOptional: true);
        private readonly InputTreeString _inViewAttributes = new InputTreeString(ParamInfos.SectionViewAttributes, isOptional: true);
        private readonly InputTreeString _inMarkAttributes = new InputTreeString(ParamInfos.SectionMarkAttributes, isOptional: true);
        private readonly InputTreeNumber _inScale = new InputTreeNumber(ParamInfos.Scale, isOptional: true);
        private readonly InputTreeString _inName = new InputTreeString(ParamInfos.Name, isOptional: true);

        private readonly OutputTreeParam<DatabaseObject> _outView = new OutputTreeParam<DatabaseObject>(ParamInfos.View, 0);
        private readonly OutputTreeParam<DatabaseObject> _outMark = new OutputTreeParam<DatabaseObject>(ParamInfos.CurvedSectionMark, 1);

        internal (ViewCollection<View> views, TreeData<Point3d> startPoints, TreeData<Point3d> midPoints, TreeData<Point3d> endPoints, TreeData<Point3d> insertPoints, TreeData<double> depthsUp, TreeData<double> depthsDown, TreeData<string> viewAttributes, TreeData<string> markAttributes, TreeData<double> scales, TreeData<string> names) GetInputValues()
        {
            return (new ViewCollection<View>(_inView.Value),
                    _inStartPoint.AsTreeData(),
                    _inMidPoint.AsTreeData(),
                    _inEndPoint.AsTreeData(),
                    _inInsertionPoint.AsTreeData(),
                    _inDepthUp.IsEmpty() ? _inDepthUp.GetDefault(_defaultDepth) : _inDepthUp.AsTreeData(),
                    _inDepthDown.IsEmpty() ? _inDepthDown.GetDefault(_defaultDepth) : _inDepthDown.AsTreeData(),
                    _inViewAttributes.IsEmpty() ? _inViewAttributes.GetDefault(_defaultAttributes) : _inViewAttributes.AsTreeData(),
                    _inMarkAttributes.IsEmpty() ? _inMarkAttributes.GetDefault(_defaultAttributes) : _inMarkAttributes.AsTreeData(),
                    _inScale.IsEmpty() ? _inScale.GetDefault(0) : _inScale.AsTreeData(),
                    _inName.IsEmpty() ? _inName.GetDefault(string.Empty) : _inName.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure views, IGH_Structure marks)
        {
            _outView.Value = views;
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
