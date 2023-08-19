using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementAttributesGoo : GH_Goo<ReinforcementBase.ReinforcementSingleAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla reinforcement attributes";

        public override string TypeName => typeof(ReinforcementBase.ReinforcementSingleAttributes).ToShortString();

        public ReinforcementAttributesGoo()
        {
        }

        public ReinforcementAttributesGoo(ReinforcementBase.ReinforcementSingleAttributes attr)
            : base(attr)
        {
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is ReinforcementBase.ReinforcementSingleAttributes)
            {
                Value = source as ReinforcementBase.ReinforcementSingleAttributes;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return ReflectionHelper.GetPropertiesWithValues(Value);
        }
    }
}
