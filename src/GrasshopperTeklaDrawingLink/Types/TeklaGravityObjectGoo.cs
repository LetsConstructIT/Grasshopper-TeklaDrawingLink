using Grasshopper.Kernel.Types;
using Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaGravityObjectGoo : GH_Goo<ModelObject>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                {
                    return typeof(Part).ToString();
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
                    return typeof(Part).ToShortString();
                }
                return Value.GetType().ToShortString();
            }
        }

        public TeklaGravityObjectGoo()
        {

        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            var input = source;
            if (source is TeklaDatabaseObjectGoo)
            {
                source = (source as TeklaDatabaseObjectGoo).Value;
            }

            if (source is TSD.Part)
            {
                var drawingPart = source as TSD.Part;
                input = ModelInteractor.GetModelObject(drawingPart.ModelIdentifier);
            }

            if (input is Part)
            {
                Value = input as Part;
                return true;
            }
            else if (input is Assembly)
            {
                Value = input as Assembly;
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
            {
                return "No value";
            }

            return Value.GetType().ToShortString();
        }
    }
}