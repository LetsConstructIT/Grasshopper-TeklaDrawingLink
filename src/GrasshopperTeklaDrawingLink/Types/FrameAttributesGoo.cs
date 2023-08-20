using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FrameAttributesGoo : GH_Goo<Frame>
    {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla frame attributes";

        public override string TypeName => typeof(Frame).ToShortString();

        public FrameAttributesGoo()
        {
        }

        public FrameAttributesGoo(Frame attr)
            : base(attr)
        {
        }
        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is Frame)
            {
                Value = source as Frame;
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
