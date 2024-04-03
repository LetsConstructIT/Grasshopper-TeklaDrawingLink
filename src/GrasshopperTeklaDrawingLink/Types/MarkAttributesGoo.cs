using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class MarkAttributesGoo : TeklaAttributesBaseGoo<Mark.MarkAttributes>
    {
        public MarkAttributesGoo()
        {

        }
        public MarkAttributesGoo(Mark.MarkAttributes attributes) : base(attributes)
        {
        }
    }
    public class MarkBaseAttributesGoo : TeklaAttributesBaseGoo<MarkBase.MarkBaseAttributes>
    {
        public MarkBaseAttributesGoo()
        {

        }
        public MarkBaseAttributesGoo(MarkBase.MarkBaseAttributes attributes) : base(attributes)
        {
        }
    }
}
