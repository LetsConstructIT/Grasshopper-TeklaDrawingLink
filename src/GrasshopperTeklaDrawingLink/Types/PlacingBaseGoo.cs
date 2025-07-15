using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PlacingBaseGoo : TeklaAttributesBaseGoo<PlacingBase>
    {
        public PlacingBaseGoo()
        {

        }
        public PlacingBaseGoo(PlacingBase placingBase)
        {
            Value = placingBase;
        }

        public override IGH_Goo Duplicate()
        {
            return new PlacingBaseGoo(Value);
        }

        public override string ToString()
        {
            return $"{Value.GetType()} {base.ToString()}";
        }
    }
}
