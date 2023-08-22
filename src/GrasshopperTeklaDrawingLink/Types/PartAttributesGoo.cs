using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PartAttributesGoo : GH_Goo<Part.PartAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla part attributes";

        public override string TypeName => typeof(Part.PartAttributes).ToShortString();

        public PartAttributesGoo()
        {
        }

        public PartAttributesGoo(Part.PartAttributes attributes)
            : base(attributes)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is Part.PartAttributes)
            {
                Value = source as Part.PartAttributes;
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
