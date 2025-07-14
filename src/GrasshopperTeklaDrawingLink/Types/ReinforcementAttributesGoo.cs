using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementAttributesGoo : TeklaAttributesBaseGoo<ReinforcementBase.ReinforcementSingleAttributes>
    {
        public ReinforcementAttributesGoo() { }

        public ReinforcementAttributesGoo(ReinforcementBase.ReinforcementSingleAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new ReinforcementAttributesGoo(Value);
        }
    }
}
