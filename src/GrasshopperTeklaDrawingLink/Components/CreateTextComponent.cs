using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateTextComponent : CreateDatabaseObjectComponentBaseNew<CreateTextCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.CreateText;

        public CreateTextComponent() : base(ComponentInfos.CreateTextComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (TSD.ViewBase view, List<string> texts, List<Point3d> insertionPoints, List<Point3d> leaderLineEndPoints, List<TSD.Text.TextAttributes> textAttributes) = _command.GetInputValues();

            var createdTexts = new List<TSD.Text>();

            var count = new int[] { texts.Count, insertionPoints.Count, leaderLineEndPoints.Count, textAttributes.Count }.Max();
            for (int i = 0; i < count; i++)
            {
                var text = texts.ElementAtOrLast(i);
                var insertionPt = insertionPoints.ElementAtOrLast(i);
                var attributes = textAttributes.ElementAtOrLast(i);

                var createdText = leaderLineEndPoints.Any() ?
                    InsertTextWithLeaderLine(view, attributes, insertionPt, leaderLineEndPoints.ElementAtOrLast(i), text) :
                    InsertTextWithoutLeaderLine(view, attributes, insertionPt, text);

                createdTexts.Add(createdText);
            }


            _command.SetOutputValues(DA, createdTexts);

            DrawingInteractor.CommitChanges();
            return createdTexts;
        }

        private TSD.Text InsertTextWithoutLeaderLine(TSD.ViewBase view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, string text)
        {
            var textElement = new TSD.Text(view, insertionPoint.ToTekla(), text, attribute)
            {
                Placing = TSD.PlacingTypes.PointPlacing()
            };

            textElement.Insert();
            return textElement;
        }

        private TSD.Text InsertTextWithLeaderLine(TSD.ViewBase view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, Point3d leaderLinePoint, string text)
        {
            var textElement = new TSD.Text(view, insertionPoint.ToTekla(), text, attribute)
            {
                Placing = TSD.PlacingTypes.LeaderLinePlacing(leaderLinePoint.ToTekla())
            };

            textElement.Insert();
            return textElement;
        }
    }

    public class CreateTextCommand : CommandBase
    {
        private readonly InputParam<TSD.ViewBase> _inView = new InputParam<TSD.ViewBase>(ParamInfos.ViewBase);
        private readonly InputListParam<string> _inText = new InputListParam<string>(ParamInfos.Text);
        private readonly InputListPoint _inInsertionPoints = new InputListPoint(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListPoint _inLeaderLineEndPoints = new InputOptionalListPoint(ParamInfos.MarkLeaderLineEndPoint, new List<Point3d>());
        private readonly InputListParam<TSD.Text.TextAttributes> _inAttributes = new InputListParam<TSD.Text.TextAttributes>(ParamInfos.TextAttributes);

        private readonly OutputListParam<TSD.Text> _outText = new OutputListParam<TSD.Text>(ParamInfos.Text);

        internal (TSD.ViewBase view, List<string> texts, List<Point3d> insertionPoints, List<Point3d> leaderLineEndPoints, List<TSD.Text.TextAttributes> textAttributes) GetInputValues()
        {
            return (
                _inView.Value,
                _inText.Value,
                _inInsertionPoints.Value,
                _inLeaderLineEndPoints.Value,
                _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSD.Text> texts)
        {
            _outText.Value = texts;

            return SetOutput(DA);
        }
    }
}
