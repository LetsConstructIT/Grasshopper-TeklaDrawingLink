using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class FrameAttributesGoo : TeklaAttributesBaseGoo<Frame>
    {
        public FrameAttributesGoo() { }

        public FrameAttributesGoo(Frame attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new FrameAttributesGoo(Value);
        }
    }
}
