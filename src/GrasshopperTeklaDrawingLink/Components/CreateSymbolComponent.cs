using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateSymbolComponent : CreateDatabaseObjectComponentBaseNew<CreateSymbolCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.Symbol;

        public CreateSymbolComponent() : base(ComponentInfos.CreateSymbolComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (viewBase, points, symbolAttributes) = _command.GetInputValues();

            var createdSymbols = new List<Symbol>();

            var count = new int[] { points.Count, symbolAttributes.Count }.Max();
            for (int i = 0; i < count; i++)
            {
                var point = points.ElementAtOrLast(i);
                var attributes = symbolAttributes.ElementAtOrLast(i);

                var symbol = new Symbol(
                    viewBase,
                    point.ToTekla(),
                    attributes.SymbolInfo,
                    attributes.Attributes);
                symbol.Insert();

                createdSymbols.Add(symbol);
            }


            _command.SetOutputValues(DA, createdSymbols);

            DrawingInteractor.CommitChanges();
            return createdSymbols;
        }
    }

    public class CreateSymbolCommand : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputListPoint _inPoints = new InputListPoint(ParamInfos.InsertionPoint);
        private readonly InputListParam<SymbolAttributes> _inAttributes = new InputListParam<SymbolAttributes>(ParamInfos.SymbolAtributes);

        private readonly OutputListParam<Symbol> _outSymbol = new OutputListParam<Symbol>(ParamInfos.Symbol);

        internal (ViewBase view, List<Point3d> points, List<SymbolAttributes> attributes) GetInputValues()
        {
            return (
                _inView.Value,
                _inPoints.Value,
                _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Symbol> symbols)
        {
            _outSymbol.Value = symbols;

            return SetOutput(DA);
        }
    }
}
