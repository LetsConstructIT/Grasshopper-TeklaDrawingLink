using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Views
{
    public class CreateDetailViewComponent : CreateViewBaseComponentNew<CreateDetailViewCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.DetailView;

        public CreateDetailViewComponent() : base(ComponentInfos.CreateDetailViewComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (inputViews, centerPoints, radiuses, labelPoints, insertPoints, viewAttributes, markAttributes, scales, names) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, centerPoints, radiuses, labelPoints, insertPoints, viewAttributes, markAttributes, scales, names);
            var inputMode = strategy.Mode;

            var outputViewTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputMarkTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                (View view, DetailMark mark) = InsertView(views.Get(path),
                                                          centerPoints.Get(i, inputMode),
                                                          radiuses.Get(i, inputMode),
                                                          labelPoints.Get(i, inputMode),
                                                          insertPoints.Get(i, inputMode),
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

        private (View view, DetailMark mark) InsertView(View view,
                                                        Point3d centerPoint,
                                                        double radius,
                                                        Point3d labelPoint,
                                                        Point3d insertionPoint,
                                                        string viewAttributesFileName,
                                                        string markAttributesFileName,
                                                        double scale,
                                                        string viewName)
        {
            var markAttributes = new DetailMark.DetailMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            var viewAttributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(viewAttributesFileName))
                viewAttributes.LoadAttributes(viewAttributesFileName);
            if (scale > 1)
                viewAttributes.Scale = scale;

            var distance = radius / Math.Sqrt(2);
            var boundaryPoint = centerPoint + new Point3d(distance, distance, 0);
            View.CreateDetailView(
                view,
                centerPoint.ToTekla(),
                boundaryPoint.ToTekla(),
                labelPoint.ToTekla(),
                insertionPoint.ToTekla(),
                viewAttributes,
                markAttributes,
                out View createdView,
                out DetailMark createdMark);

            var macroApplied = LoadAttributesWithMacroIfNecessary(createdView, viewAttributesFileName);

            if (macroApplied && !string.IsNullOrEmpty(viewName))
            {
                createdMark.Attributes.MarkName = viewName;
                createdMark.Modify();
            }

            return (createdView, createdMark);
        }
    }

    public class CreateDetailViewCommand : CommandBase
    {
        private const double _defaultRadius = 500;
        private const string _defaultAttributes = "standard";

        private readonly InputOptionalListParam<View> _inView = new InputOptionalListParam<View>(ParamInfos.View);

        private readonly InputTreePoint _inCenterPoint = new InputTreePoint(ParamInfos.DetailCenterPoint);
        private readonly InputTreeNumber _inRadius = new InputTreeNumber(ParamInfos.DetailRadius, isOptional: true);
        private readonly InputTreePoint _inLabelPoint = new InputTreePoint(ParamInfos.DetailLabelPoint);
        private readonly InputTreePoint _inInsertionPoint = new InputTreePoint(ParamInfos.DetailInsertionPoint);
        private readonly InputTreeString _inViewAttributes = new InputTreeString(ParamInfos.DetailViewAttributes, isOptional: true);
        private readonly InputTreeString _inMarkAttributes = new InputTreeString(ParamInfos.DetailMarkAttributes, isOptional: true);
        private readonly InputTreeNumber _inScale = new InputTreeNumber(ParamInfos.Scale, isOptional: true);
        private readonly InputTreeString _inName = new InputTreeString(ParamInfos.Name, isOptional: true);

        private readonly OutputTreeParam<DatabaseObject> _outView = new OutputTreeParam<DatabaseObject>(ParamInfos.View, 0);
        private readonly OutputTreeParam<DatabaseObject> _outMark = new OutputTreeParam<DatabaseObject>(ParamInfos.Mark, 1);

        internal (List<View> views, TreeData<Point3d> centerPoints, TreeData<double> radiuses, TreeData<Point3d> labelPoints, TreeData<Point3d> insertPoints, TreeData<string> viewAttributes, TreeData<string> markAttributes, TreeData<double> scales, TreeData<string> names) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                    _inCenterPoint.AsTreeData(),
                    _inRadius.IsEmpty() ? _inRadius.GetDefault(_defaultRadius) : _inRadius.AsTreeData(),
                    _inLabelPoint.AsTreeData(),
                    _inInsertionPoint.AsTreeData(),
                    _inViewAttributes.IsEmpty() ? _inViewAttributes.GetDefault(_defaultAttributes) : _inViewAttributes.AsTreeData(),
                    _inMarkAttributes.IsEmpty() ? _inMarkAttributes.GetDefault(_defaultAttributes) : _inMarkAttributes.AsTreeData(),
                    _inScale.IsEmpty() ? _inScale.GetDefault(0) : _inScale.AsTreeData(),
                    _inName.IsEmpty() ? _inName.GetDefault(string.Empty) : _inName.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure views, IGH_Structure marks)
        {
            _outView.Value = views;
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
