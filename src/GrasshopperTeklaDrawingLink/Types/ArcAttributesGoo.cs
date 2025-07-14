using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ArcAttributesGoo : TeklaAttributesBaseGoo<Arc.ArcAttributes>
    {
        public ArcAttributesGoo() { }

        public ArcAttributesGoo(Arc.ArcAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new ArcAttributesGoo(Value);
        }
    }
}
