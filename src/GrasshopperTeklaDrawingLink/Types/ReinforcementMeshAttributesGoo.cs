using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ReinforcementMeshAttributesGoo : GH_Goo<ReinforcementBase.ReinforcementMeshAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla reinforcement mesh attributes";

        public override string TypeName => typeof(ReinforcementBase.ReinforcementMeshAttributes).ToShortString();

        public ReinforcementMeshAttributesGoo()
        {
        }

        public ReinforcementMeshAttributesGoo(ReinforcementBase.ReinforcementMeshAttributes attr)
            : base(attr)
        {
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is ReinforcementBase.ReinforcementMeshAttributes)
            {
                Value = source as ReinforcementBase.ReinforcementMeshAttributes;
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
