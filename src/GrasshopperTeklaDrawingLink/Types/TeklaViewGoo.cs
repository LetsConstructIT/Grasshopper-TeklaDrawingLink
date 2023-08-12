using System;
using Tekla.Structures.Model.UI;
using Grasshopper.Kernel.Types;

namespace GTDrawingLink.Types
{
    public class TeklaViewGoo : GH_Goo<TeklaView>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla View";

        public override string TypeName => typeof(TeklaView).ToShortString();

        public TeklaViewGoo()
        {
        }

        public TeklaViewGoo(TeklaView attr)
            : base(attr)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is TeklaView)
            {
                Value = source as TeklaView;
                return true;
            }
            else if (source is View view)
            {
                Value = new TeklaView(view);
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return $"{Value.Name}";
        }
    }
}
