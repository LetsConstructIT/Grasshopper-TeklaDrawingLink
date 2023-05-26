using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDatabaseObjectGoo : GH_Goo<DatabaseObject>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                {
                    return typeof(DatabaseObject).ToString();
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
                    return typeof(DatabaseObject).ToShortString();
                }
                return Value.GetType().ToShortString();
            }
        }

        public TeklaDatabaseObjectGoo()
        {
        }

        public TeklaDatabaseObjectGoo(DatabaseObject modelObject)
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
            if (source is DatabaseObject)
            {
                Value = source as DatabaseObject;
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