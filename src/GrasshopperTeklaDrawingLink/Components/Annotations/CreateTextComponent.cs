using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateTextComponent : CreateDatabaseObjectComponentBaseNew<CreateTextCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.CreateText;

        public CreateTextComponent() : base(ComponentInfos.CreateTextComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var texts, var insertionPoints, var placings, var textAttributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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

            var views = new ViewCollection<ViewBase>(inputViews);
            var strategy = GetSolverStrategy(false, texts, insertionPoints, placings, textAttributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertText(views.Get(path),
                                      textAttributes.Get(i, inputMode),
                                      insertionPoints.Get(i, inputMode),
                                      placings.Get(i, inputMode),
                                      texts.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private Text InsertText(ViewBase view, Text.TextAttributes attribute, Point3d insertionPoint, PlacingBase placing, string text)
        {
            var textElement = new Text(view, insertionPoint.ToTekla(), text, attribute)
            {
                Placing = placing
            };

            textElement.Insert();
            return textElement;
        }
    }

    public class CreateTextCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeString _inText = new InputTreeString(ParamInfos.Text, isOptional: true);
        private readonly InputTreePoint _inInsertionPoints = new InputTreePoint(ParamInfos.MarkInsertionPoint, isOptional: true);
        private readonly InputOptionalTreeParam<PlacingBase> _inPlacingTypes = new InputOptionalTreeParam<PlacingBase>(ParamInfos.PlacingType);
        private readonly InputTreeParam<Text.TextAttributes> _inAttributes = new InputTreeParam<Text.TextAttributes>(ParamInfos.TextAttributes);

        private readonly OutputTreeParam<TSD.Text> _outText = new OutputTreeParam<TSD.Text>(ParamInfos.Text, 0);

        internal (List<ViewBase> views, TreeData<string> texts, TreeData<Point3d> insertionPoints, TreeData<PlacingBase> placings, TreeData<Text.TextAttributes> textAttributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                _inText.AsTreeData(),
                _inInsertionPoints.AsTreeData(),
                _inPlacingTypes.AsTreeData(),
                _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems() && result.Item4.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure texts)
        {
            _outText.Value = texts;

            return SetOutput(DA);
        }
    }
}
