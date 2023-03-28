using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingGoo : GH_Goo<Drawing>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                    return typeof(Drawing).ToString();

                return Value.GetType().ToString();
            }
        }

        public override string TypeName
        {
            get
            {
                if (Value == null)
                    return typeof(Drawing).ToShortString();

                return Value.GetType().ToShortString();
            }
        }

        public TeklaDrawingGoo()
        {
        }

        public TeklaDrawingGoo(Drawing drawing)
            : base(drawing)
        {
            Value = drawing;
        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            if (source is Drawing)
            {
                Value = source as Drawing;
                return true;
            }

            return base.CastFrom(source);
        }

        public override bool CastTo<Q>(ref Q target)
        {
            return base.CastTo(ref target);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            var type = Value.GetType().Name.Replace("Tekla.Structures.Drawing.", "");

            return $"{type}, name: {Value.Name}";
        }
    }
}