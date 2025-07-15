﻿using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolAttributesGoo : TeklaAttributesBaseGoo<SymbolAttributes>
    {
        public SymbolAttributesGoo() { }

        public SymbolAttributesGoo(SymbolAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new SymbolAttributesGoo(Value);
        }

        public override bool CastFrom(object source)
        {
            if (source is SymbolAttributes)
            {
                Value = source as SymbolAttributes;
                return true;
            }
            else if (source is Symbol.SymbolAttributes)
            {
                Value = new SymbolAttributes() { Attributes = source as Symbol.SymbolAttributes };
                return true;
            }
            return base.CastFrom(source);
        }
    }
}
