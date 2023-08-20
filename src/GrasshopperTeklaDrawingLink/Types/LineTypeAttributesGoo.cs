using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class LineTypeAttributesGoo : GH_Goo<LineTypeAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla line type attributes";

        public override string TypeName => typeof(LineTypeAttributes).ToShortString();

        public LineTypeAttributesGoo()
        {
        }

        public LineTypeAttributesGoo(LineTypeAttributes attr)
            : base(attr)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is LineTypeAttributes)
            {
                Value = source as LineTypeAttributes;
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
