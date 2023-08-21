using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolAttributes
    {
        public Symbol.SymbolAttributes Attributes { get; set; }
        public SymbolInfo SymbolInfo { get; set; }

        public SymbolAttributes()
        {
            Attributes = new Symbol.SymbolAttributes();
            SymbolInfo = new SymbolInfo();
        }
    }
}
