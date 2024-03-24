using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class ModifyDrawingPropertiesComponent : TeklaComponentBaseNew<ModifyDrawingPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingProperties;

        public ModifyDrawingPropertiesComponent() : base(ComponentInfos.ModifyDrawingPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (Drawing drawing, string? name, string? title1, string? title2, string? title3) = _command.GetInputValues();
            drawing.Select();

            var needsModification = false;
            if (!string.IsNullOrEmpty(name))
            {
                drawing.Name = name;
                needsModification = true;
            }
            if (!string.IsNullOrEmpty(title1))
            {
                drawing.Title1 = title1;
                needsModification = true;
            }
            if (!string.IsNullOrEmpty(title2))
            {
                drawing.Title2 = title2;
                needsModification = true;
            }
            if (!string.IsNullOrEmpty(title3))
            {
                drawing.Title3 = title3;
                needsModification = true;
            }

            if (needsModification)
                drawing.Modify();

            _command.SetOutputValues(DA, drawing, drawing.Name, drawing.Title1, drawing.Title2, drawing.Title3, GetSheetNumber(drawing));
        }

        private int GetSheetNumber(Drawing drawing)
        {
            return drawing switch
            {
                CastUnitDrawing cuDrawing => cuDrawing.SheetNumber,
                AssemblyDrawing assemblyDrawing => assemblyDrawing.SheetNumber,
                SinglePartDrawing singlePartDrawing => singlePartDrawing.SheetNumber,
                _ => 0
            };
        }
    }

    public class ModifyDrawingPropertiesCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly InputOptionalParam<string> _inName = new InputOptionalParam<string>(ParamInfos.DrawingName);
        private readonly InputOptionalParam<string> _inTitle1 = new InputOptionalParam<string>(ParamInfos.DrawingTitle1);
        private readonly InputOptionalParam<string> _inTitle2 = new InputOptionalParam<string>(ParamInfos.DrawingTitle2);
        private readonly InputOptionalParam<string> _inTitle3 = new InputOptionalParam<string>(ParamInfos.DrawingTitle3);

        private readonly OutputParam<Drawing> _outDrawing = new OutputParam<Drawing>(ParamInfos.Drawing);
        private readonly OutputParam<string> _outName = new OutputParam<string>(ParamInfos.DrawingName);
        private readonly OutputParam<string> _outTitle1 = new OutputParam<string>(ParamInfos.DrawingTitle1);
        private readonly OutputParam<string> _outTitle2 = new OutputParam<string>(ParamInfos.DrawingTitle2);
        private readonly OutputParam<string> _outTitle3 = new OutputParam<string>(ParamInfos.DrawingTitle3);
        private readonly OutputParam<int> _outSheetNumber = new OutputParam<int>(ParamInfos.DrawingSheetNumber);

        internal (Drawing drawing, string? name, string? title1, string? title2, string? title3) GetInputValues()
        {
            return (_inDrawing.Value,
                _inName.GetValueFromUserOrNull(),
                _inTitle1.GetValueFromUserOrNull(),
                _inTitle2.GetValueFromUserOrNull(),
                _inTitle3.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Drawing drawing, string name, string title1, string title2, string title3, int sheetNumber)
        {
            _outDrawing.Value = drawing;
            _outName.Value = name;
            _outTitle1.Value = title1;
            _outTitle2.Value = title2;
            _outTitle3.Value = title3;
            _outSheetNumber.Value = sheetNumber;

            return SetOutput(DA);
        }
    }
}
