using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Model;

namespace GTDrawingLink.Types
{
    public class ModelObjectGoo : GH_Goo<ModelObject>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla model object";

        public override string TypeName => typeof(ModelObject).ToShortString();

        public ModelObjectGoo()
        {
        }

        public ModelObjectGoo(ModelObject modelObject)
            : base(modelObject)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is ModelObject)
            {
                Value = source as ModelObject;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return Value.GetType().ToShortString();
        }
    }
}
