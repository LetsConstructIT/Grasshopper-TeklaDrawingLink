using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class TextAttributesGoo : GH_Goo<Text.TextAttributes> {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla text attributes";

        public override string TypeName => typeof(Text.TextAttributes).ToShortString();

        public TextAttributesGoo() {
        }

        public TextAttributesGoo(Text.TextAttributes attr)
            : base(attr) {
        }
        public override IGH_Goo Duplicate() {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source) {
            if(source is Text.TextAttributes) {
                Value=source as Text.TextAttributes;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString() {
            if(Value==null)
                return "No value";

            return ReflectionHelper.GetPropertiesWithValues(Value);
        }
    }
}
