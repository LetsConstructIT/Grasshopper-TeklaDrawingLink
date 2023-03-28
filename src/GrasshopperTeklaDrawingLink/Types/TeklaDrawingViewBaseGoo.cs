using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingViewBaseGoo : GH_Goo<ViewBase>
    {
        public override bool IsValid => true;

        public override string TypeDescription
        {
            get
            {
                if (Value == null)
                    return typeof(ViewBase).ToString();

                return Value.GetType().ToString();
            }
        }

        public override string TypeName
        {
            get
            {
                if (Value == null)
                    return typeof(ViewBase).ToShortString();

                return Value.GetType().ToShortString();
            }
        }

        public TeklaDrawingViewBaseGoo()
        {
        }

        public TeklaDrawingViewBaseGoo(ViewBase view)
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
            if (source is ViewBase)
            {
                Value = source as ViewBase;
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

            if (Value is View)
            {
                var view = Value as View;
                return $"View: {view.Name}, type: {view.ViewType}";
            }
            else
            {
                return $"View frame. Height: {Value.Height}, width: {Value.Width}";
            }
        }
    }
}