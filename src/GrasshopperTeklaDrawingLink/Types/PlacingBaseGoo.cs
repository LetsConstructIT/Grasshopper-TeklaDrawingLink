using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PlacingBaseGoo : TeklaAttributesBaseGoo<PlacingBase>
    {
        public override string ToString()
        {
            return $"{Value.GetType()} {base.ToString()}";
        }
    }
}
