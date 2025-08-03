using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolylineAttributesGoo : TeklaAttributesBaseGoo<Polyline.PolylineAttributes>
    {
        public PolylineAttributesGoo() { }

        public PolylineAttributesGoo(Polyline.PolylineAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new PolylineAttributesGoo(Value);
        }
    }
}
