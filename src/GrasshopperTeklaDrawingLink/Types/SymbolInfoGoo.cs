using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolInfoGoo : TeklaAttributesBaseGoo<SymbolInfo>
    {
        public SymbolInfoGoo() { }

        public SymbolInfoGoo(SymbolInfo attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new SymbolInfoGoo(Value);
        }
    }
}
