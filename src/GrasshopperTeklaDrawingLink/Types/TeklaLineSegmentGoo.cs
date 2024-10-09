using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaLineSegmentGoo : GH_Goo<LineSegment>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                {
                    return typeof(LineSegment).ToString();
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
                    return typeof(LineSegment).ToShortString();
                }
                return Value.GetType().ToShortString();
            }
        }

        public TeklaLineSegmentGoo()
        {
        }

        public TeklaLineSegmentGoo(LineSegment segment)
            : base(segment)
        {
            Value = segment;
        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            if (source is GH_Line)
            {
                Value = (source as GH_Line).Value.ToTeklaSegment();
                return true;
            }
            else if (source is LineSegment segment)
            {
                Value = segment;
                return true;
            }
            return base.CastFrom(source);
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (target is IGH_GeometricGoo && Value != null)
            {
                target = (Q)(object)new GH_Line(Value.ToRhino());
                return true;
            }
            return base.CastTo(ref target);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return $"Line: {Value.Point1} {Value.Point2}";
        }
    }
}