using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class ModelObjectHatchAttributesGoo : TeklaAttributesBaseGoo<ModelObjectHatchAttributes>
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
            if (Value == null)
                throw new NotImplementedException();

            var type = typeof(ModelObjectHatchAttributes);
            var duplicate = (ModelObjectHatchAttributes)type.GetConstructor(
                  BindingFlags.NonPublic | BindingFlags.Instance,
                  null, Type.EmptyTypes, null).Invoke(null);

            foreach (var hatchProperty in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!hatchProperty.CanRead || !hatchProperty.CanWrite)
                    continue;

                var existingValue = hatchProperty.GetValue(Value);
                hatchProperty.SetValue(duplicate, existingValue);
            }

            return new ModelObjectHatchAttributesGoo(duplicate);
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
