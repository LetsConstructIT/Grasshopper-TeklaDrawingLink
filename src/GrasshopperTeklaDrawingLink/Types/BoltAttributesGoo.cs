using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class BoltAttributesGoo : TeklaAttributesBaseGoo<Bolt.BoltAttributes>
    {
        public BoltAttributesGoo() { }

        public BoltAttributesGoo(Bolt.BoltAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new BoltAttributesGoo(Value);
        }
    }
}
