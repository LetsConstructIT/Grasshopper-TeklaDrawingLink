using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementMeshAttributesGoo : TeklaAttributesBaseGoo<ReinforcementBase.ReinforcementMeshAttributes>
    {
        public ReinforcementMeshAttributesGoo() { }

        public ReinforcementMeshAttributesGoo(ReinforcementBase.ReinforcementMeshAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new ReinforcementMeshAttributesGoo(Value);
        }
    } 
}
