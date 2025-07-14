using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PartAttributesGoo : TeklaAttributesBaseGoo<Part.PartAttributes>
    {
        public PartAttributesGoo() { }

        public PartAttributesGoo(Part.PartAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new PartAttributesGoo(Value);
        }
    }
}
