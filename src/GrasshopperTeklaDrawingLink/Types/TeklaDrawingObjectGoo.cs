using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingObjectGoo : GH_Goo<DrawingObject>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                {
                    return typeof(DrawingObject).ToString();
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
                    return typeof(DrawingObject).ToShortString();
                }
                return Value.GetType().ToShortString();
            }
        }

        public TeklaDrawingObjectGoo()
        {
        }

        public TeklaDrawingObjectGoo(DrawingObject modelObject)
            : base(modelObject)
        {
            Value = modelObject;
        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            if (source is DrawingObject)
            {
                Value = source as DrawingObject;
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