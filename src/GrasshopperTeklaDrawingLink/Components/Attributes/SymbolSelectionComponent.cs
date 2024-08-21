using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Attributes
{
    public class SymbolSelectionComponent : TeklaComponentBaseNew<SymbolSelectionCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.SymbolSelection;

        public SymbolSelectionComponent() : base(ComponentInfos.SymbolSelectionComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (string file, int index) = _command.GetInputValues();

            var symbolInfo = new SymbolInfo(file, index);

            _command.SetOutputValues(DA, symbolInfo);
        }
    }

    public class SymbolSelectionCommand : CommandBase
    {
        private readonly InputOptionalParam<string> _inFile = new InputOptionalParam<string>(ParamInfos.SymbolFile, "xsteel");
        private readonly InputOptionalStructParam<int> _inIndex = new InputOptionalStructParam<int>(ParamInfos.SymbolIndex, 0);

        private readonly OutputParam<SymbolInfo> _outAttributes = new OutputParam<SymbolInfo>(ParamInfos.SymbolSelection);

        internal (string file, int index) GetInputValues()
        {
            return (
                _inFile.Value,
                _inIndex.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, SymbolInfo symbolInfo)
        {
            _outAttributes.Value = symbolInfo;

            return SetOutput(DA);
        }
    }
}
