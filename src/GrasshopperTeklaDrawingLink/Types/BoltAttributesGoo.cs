using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class BoltAttributesGoo : GH_Goo<Bolt.BoltAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla bolt attributes";

        public override string TypeName => typeof(Bolt.BoltAttributes).ToShortString();

        public BoltAttributesGoo()
        {
        }

        public BoltAttributesGoo(Bolt.BoltAttributes attributes)
            : base(attributes)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is Bolt.BoltAttributes)
            {
                Value = source as Bolt.BoltAttributes;
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
