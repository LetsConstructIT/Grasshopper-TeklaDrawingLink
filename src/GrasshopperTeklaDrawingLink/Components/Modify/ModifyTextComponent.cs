using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.ModifyComponents
{
    public class ModifyTextComponent : TeklaComponentBaseNew<ModifyTextCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.ModifyText;
        public ModifyTextComponent() : base(ComponentInfos.ModifyTextComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (var texts, var contents, var insertionPts, var placings, var attributes) = _command.GetInputValues();

            for (int i = 0; i < texts.Count; i++)
            {
                var text = texts[i];

                if (contents.HasItems())
                {
                    var content = contents.ElementAtOrLast(i);
                    if (!string.IsNullOrEmpty(content))
                        text.TextString = content;
                }

                if (attributes.HasItems())
                {
                    var attribute = attributes.ElementAtOrLast(i);
                    if (attribute != null)
                        text.Attributes = attribute;
                }

                if (insertionPts.HasItems())
                {
                    var insertionPt = insertionPts.ElementAtOrLast(i);
                    if (insertionPt != null)
                    {
                        text.InsertionPoint = insertionPt.Value.ToTekla();
                        text.Attributes.PlacingAttributes.IsFixed = true;
                    }
                }

                if (placings.HasItems())
                {
                    var placing = placings.ElementAtOrLast(i);
                    if (placing != null)
                    {
                        text.Placing = placing;
                        text.Attributes.PlacingAttributes.IsFixed = true;
                    }
                }

                text.Modify();
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, texts);
        }
    }

    public class ModifyTextCommand : CommandBase
    {
        private readonly InputListParam<Text> _inTexts = new InputListParam<Text>(ParamInfos.Text);
        private readonly InputOptionalListParam<string> _inContents = new InputOptionalListParam<string>(ParamInfos.TextContents);
        private readonly InputOptionalListParam<GH_Point> _insertionPts = new InputOptionalListParam<GH_Point>(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListParam<PlacingBase> _inPlacings = new InputOptionalListParam<PlacingBase>(ParamInfos.PlacingType);
        private readonly InputOptionalListParam<Text.TextAttributes> _inAttributes = new InputOptionalListParam<Text.TextAttributes>(ParamInfos.TextAttributes);

        private readonly OutputListParam<Text> _outTexts = new OutputListParam<Text>(ParamInfos.Text);

        internal (List<Text> reinforcements, List<string>? contents, List<GH_Point>? insertionPoints, List<PlacingBase>? placings, List<Text.TextAttributes>? attributes) GetInputValues()
        {
            return (
                _inTexts.Value,
                _inContents.GetValueFromUserOrNull(),
                _insertionPts.GetValueFromUserOrNull(),
                _inPlacings.GetValueFromUserOrNull(),
                _inAttributes.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Text> texts)
        {
            _outTexts.Value = texts;

            return SetOutput(DA);
        }
    }
}
