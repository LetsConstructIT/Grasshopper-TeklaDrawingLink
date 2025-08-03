using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolygonAttributesGoo : TeklaAttributesBaseGoo<Polygon.PolygonAttributes>
    {
        public PolygonAttributesGoo() { }

        public PolygonAttributesGoo(Polygon.PolygonAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new PolygonAttributesGoo(Value);
        }
    }
}
