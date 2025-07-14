using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class WeldAttributesGoo : TeklaAttributesBaseGoo<Weld.WeldAttributes>
    {
        public WeldAttributesGoo() { }

        public WeldAttributesGoo(Weld.WeldAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new WeldAttributesGoo(Value);
        }
    }
}
