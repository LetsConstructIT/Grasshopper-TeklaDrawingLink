using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineAttributesGoo : TeklaAttributesBaseGoo<Line.LineAttributes>
    {
        public LineAttributesGoo() { }

        public LineAttributesGoo(Line.LineAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new LineAttributesGoo(Value);
        }
    }
}
