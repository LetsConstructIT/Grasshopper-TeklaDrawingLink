using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class StraightDimensionSetAttributesGoo : GH_Goo<StraightDimensionSet.StraightDimensionSetAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla straigh dimension set attributes";

        public override string TypeName => typeof(StraightDimensionSet.StraightDimensionSetAttributes).ToShortString();

        public StraightDimensionSetAttributesGoo()
        {
        }

        public StraightDimensionSetAttributesGoo(StraightDimensionSet.StraightDimensionSetAttributes attr)
            : base(attr)
        {
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is StraightDimensionSet.StraightDimensionSetAttributes)
            {
                Value = source as StraightDimensionSet.StraightDimensionSetAttributes;
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
