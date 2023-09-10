using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ModelObjectHatchAttributesGoo : GH_Goo<ModelObjectHatchAttributes>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla model object hatch attributes";

        public override string TypeName => typeof(ModelObjectHatchAttributes).ToShortString();

        public ModelObjectHatchAttributesGoo()
        {
        }

        public ModelObjectHatchAttributesGoo(ModelObjectHatchAttributes attr)
            : base(attr)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is ModelObjectHatchAttributes)
            {
                Value = source as ModelObjectHatchAttributes;
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
