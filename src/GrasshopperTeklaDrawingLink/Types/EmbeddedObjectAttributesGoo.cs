using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class EmbeddedObjectAttributesGoo : TeklaAttributesBaseGoo<EmbeddedObjectAttributes>
    {
        public EmbeddedObjectAttributesGoo() { }

        public EmbeddedObjectAttributesGoo(EmbeddedObjectAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new EmbeddedObjectAttributesGoo(Value);
        }
    }
}
