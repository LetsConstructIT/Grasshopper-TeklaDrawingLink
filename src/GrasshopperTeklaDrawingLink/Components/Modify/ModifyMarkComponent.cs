using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.ModifyComponents
{
    public class ModifyMarkComponent : TeklaComponentBaseNew<ModifyMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.ModifyMark;
        public ModifyMarkComponent() : base(ComponentInfos.ModifyMarkComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (var marks, var insertionPts, var placings, var attributes) = _command.GetInputValues();

            for (int i = 0; i < marks.Count; i++)
            {
                var mark = marks[i];
                mark.Select();

                if (attributes.HasItems())
                {
                    var attribute = attributes.ElementAtOrLast(i);
                    if (attribute != null)
                        mark.Attributes = attribute;
                }

                if (insertionPts.HasItems())
                {
                    var insertionPt = insertionPts.ElementAtOrLast(i);
                    if (insertionPt != null)
                    {
                        mark.InsertionPoint = insertionPt.Value.ToTekla();
                        mark.Attributes.PlacingAttributes.IsFixed = true;
                    }
                }

                if (placings.HasItems())
                {
                    var placing = placings.ElementAtOrLast(i);
                    if (placing != null)
                    {
                        mark.Placing = placing;
                        mark.Attributes.PlacingAttributes.IsFixed = true;
                    }
                }

                mark.Modify();
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, marks);
        }
    }

    public class ModifyMarkCommand : CommandBase
    {
        private readonly InputListParam<Mark> _inMarks = new InputListParam<Mark>(ParamInfos.Mark);
        private readonly InputOptionalListParam<GH_Point> _insertionPts = new InputOptionalListParam<GH_Point>(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListParam<PlacingBase> _inPlacings = new InputOptionalListParam<PlacingBase>(ParamInfos.PlacingType);
        private readonly InputOptionalListParam<Mark.MarkAttributes> _inAttributes = new InputOptionalListParam<Mark.MarkAttributes>(ParamInfos.MarkAttributes);

        private readonly OutputListParam<Mark> _outMarks = new OutputListParam<Mark>(ParamInfos.Mark);

        internal (List<Mark> marks, List<GH_Point>? insertionPoints, List<PlacingBase>? placings, List<Mark.MarkAttributes>? attributes) GetInputValues()
        {
            return (
                _inMarks.Value,
                _insertionPts.GetValueFromUserOrNull(),
                _inPlacings.GetValueFromUserOrNull(),
                _inAttributes.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Mark> marks)
        {
            _outMarks.Value = marks;

            return SetOutput(DA);
        }
    }
}
