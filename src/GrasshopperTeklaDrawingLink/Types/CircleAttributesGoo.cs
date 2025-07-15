using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class CircleAttributesGoo : TeklaAttributesBaseGoo<Circle.CircleAttributes>
    {
        public CircleAttributesGoo() { }

        public CircleAttributesGoo(Circle.CircleAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new CircleAttributesGoo(Value);
        }
    }
}
