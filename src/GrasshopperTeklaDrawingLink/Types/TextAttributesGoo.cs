using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TextAttributesGoo : TeklaAttributesBaseGoo<Text.TextAttributes>
    {
        public TextAttributesGoo() { }

        public TextAttributesGoo(Text.TextAttributes attributes)
        {
            Value = attributes;
        }
    }
}
