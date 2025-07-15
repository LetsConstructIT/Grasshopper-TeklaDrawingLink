using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FontAttributesGoo : TeklaAttributesBaseGoo<FontAttributes>
    {
        public FontAttributesGoo() { }

        public FontAttributesGoo(FontAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new FontAttributesGoo(Value);
        }
    }
}
