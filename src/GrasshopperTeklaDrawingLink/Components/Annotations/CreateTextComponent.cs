using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
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
            (var views, var texts, var insertionPoints, var placings, var textAttributes) = _command.GetInputValues();

            var strategy = GetSolverStrategy(texts, insertionPoints, placings, textAttributes);
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
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeString _inText = new InputTreeString(ParamInfos.Text);
        private readonly InputTreePoint _inInsertionPoints = new InputTreePoint(ParamInfos.MarkInsertionPoint);
        private readonly InputTreeParam<PlacingBase> _inPlacingTypes = new InputTreeParam<PlacingBase>(ParamInfos.PlacingType);
        private readonly InputTreeParam<Text.TextAttributes> _inAttributes = new InputTreeParam<Text.TextAttributes>(ParamInfos.TextAttributes);

        private readonly OutputTreeParam<TSD.Text> _outText = new OutputTreeParam<TSD.Text>(ParamInfos.Text, 0);

        internal (ViewCollection<ViewBase> views, TreeData<string> texts, TreeData<Point3d> insertionPoints, TreeData<PlacingBase> placings, TreeData<Text.TextAttributes> textAttributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                _inText.AsTreeData(),
                _inInsertionPoints.AsTreeData(),
                _inPlacingTypes.AsTreeData(),
                _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure texts)
        {
            _outText.Value = texts;

            return SetOutput(DA);
        }
    }
}
