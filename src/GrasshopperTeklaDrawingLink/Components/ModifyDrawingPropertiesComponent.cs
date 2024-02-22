using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class ModifyDrawingPropertiesComponent : TeklaComponentBaseNew<ModifyDrawingPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingProperties;

        public ModifyDrawingPropertiesComponent() : base(ComponentInfos.ModifyDrawingPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawing = _command.GetInputValues();


            var sheetNumber = GetSheetNumber(drawing);
            _command.SetOutputValues(DA, drawing.Name, drawing.Title1, drawing.Title2, drawing.Title3, sheetNumber);
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
        private readonly OutputParam<string> _outName = new OutputParam<string>(ParamInfos.DrawingName);
        private readonly OutputParam<string> _outTitle1 = new OutputParam<string>(ParamInfos.DrawingTitle1);
        private readonly OutputParam<string> _outTitle2 = new OutputParam<string>(ParamInfos.DrawingTitle2);
        private readonly OutputParam<string> _outTitle3 = new OutputParam<string>(ParamInfos.DrawingTitle3);
        private readonly OutputParam<int> _outSheetNumber = new OutputParam<int>(ParamInfos.DrawingSheetNumber);

        internal Drawing GetInputValues()
        {
            return _inDrawing.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string name, string title1, string title2, string title3, int sheetNumber)
        {
            _outName.Value = name;
            _outTitle1.Value = title1;
            _outTitle2.Value = title2;
            _outTitle3.Value = title3;
            _outSheetNumber.Value = sheetNumber;

            return SetOutput(DA);
        }
    }
}
