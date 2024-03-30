using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaPointGoo : GH_Goo<Point>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                {
                    return typeof(Point).ToString();
                }
                return Value.GetType().ToString();
            }
        }

        public override string TypeName
        {
            get
            {
                if (Value == null)
                {
                    return typeof(Point).ToShortString();
                }
                return Value.GetType().ToShortString();
            }
        }

        public TeklaPointGoo()
        {
        }

        public TeklaPointGoo(Point point)
            : base(point)
        {
            Value = point;
        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            if (source is GH_Point)
            {
                Value = (source as GH_Point).Value.ToTekla();
                return true;
            }
            return base.CastFrom(source);
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (target is IGH_GeometricGoo && Value != null)
            {
                target = (Q)(object)new GH_Point(Value.ToRhino());
                return true;
            }
            return base.CastTo(ref target);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return Value.ToString();
        }
    }
}