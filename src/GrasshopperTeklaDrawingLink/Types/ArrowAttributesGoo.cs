using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ArrowAttributesGoo : TeklaAttributesBaseGoo<ArrowheadAttributes>
    {
        public ArrowAttributesGoo() { }

        public ArrowAttributesGoo(ArrowheadAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new ArrowAttributesGoo(Value);
        }
    }
}
