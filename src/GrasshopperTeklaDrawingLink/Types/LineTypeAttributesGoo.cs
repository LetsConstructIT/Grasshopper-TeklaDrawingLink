using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineTypeAttributesGoo : TeklaAttributesBaseGoo<LineTypeAttributes>
    {
        public LineTypeAttributesGoo() { }

        public LineTypeAttributesGoo(LineTypeAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new LineTypeAttributesGoo(Value);
        }
    }
}
