using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingViewGoo : GH_Goo<View>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                    return typeof(View).ToString();

                return Value.GetType().ToString();
            }
        }

        public override string TypeName
        {
            get
            {
                if (Value == null)
                    return typeof(View).ToShortString();

                return Value.GetType().ToShortString();
            }
        }

        public TeklaDrawingViewGoo()
        {
        }

        public TeklaDrawingViewGoo(View view)
            : base(view)
        {
            Value = view;
        }

        public override IGH_Goo Duplicate()
        {
            return this;
        }

        public override bool CastFrom(object source)
        {
            if (source is View)
            {
                Value = source as View;
                return true;
            }
            else if (source is TeklaDrawingViewBaseGoo &&
                     (source as TeklaDrawingViewBaseGoo).Value is View)
            {
                Value = (source as TeklaDrawingViewBaseGoo).Value as View;
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
                return "No value";

            var view = Value as View;
            return $"View: {view.Name}, type: {view.ViewType}";
        }
    }
}