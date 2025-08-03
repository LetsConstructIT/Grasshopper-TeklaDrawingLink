using Grasshopper.Kernel.Types;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TextFileAttributesGoo : TeklaAttributesBaseGoo<TextFile.TextFileAttributes>
    {
        public TextFileAttributesGoo() { }

        public TextFileAttributesGoo(TextFile.TextFileAttributes attributes)
        {
            Value = attributes;
        }
        public override IGH_Goo Duplicate()
        {
            return new TextFileAttributesGoo(Value);
        }
    }
}
