using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolAttributesGoo : GH_Goo<Symbol.SymbolAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla symbol attributes";

        public override string TypeName => typeof(Symbol.SymbolAttributes).ToShortString();

        public SymbolAttributesGoo()
        {
        }

        public SymbolAttributesGoo(Symbol.SymbolAttributes attr)
            : base(attr)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is Symbol.SymbolAttributes)
            {
                Value = source as Symbol.SymbolAttributes;
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
