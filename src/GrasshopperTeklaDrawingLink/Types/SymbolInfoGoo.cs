using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class SymbolInfoGoo : GH_Goo<SymbolInfo>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla symbol info";

        public override string TypeName => typeof(SymbolInfo).ToShortString();

        public SymbolInfoGoo()
        {
        }

        public SymbolInfoGoo(SymbolInfo symbolInfo)
            : base(symbolInfo)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is SymbolInfo)
            {
                Value = source as SymbolInfo;
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
