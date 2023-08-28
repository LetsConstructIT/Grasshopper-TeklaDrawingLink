using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;

namespace GTDrawingLink.Types
{
    public abstract class TeklaAttributesBaseGoo<T> : GH_Goo<T> where T : class
    {
        public override bool IsValid => true;

        public override string TypeDescription => $"Tekla attributes: {nameof(T)}";

        public override string TypeName => typeof(T).ToShortString();

        public TeklaAttributesBaseGoo()
        {
        }

        public TeklaAttributesBaseGoo(T attributes) : base(attributes)
        {
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is T)
            {
                Value = source as T;
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
