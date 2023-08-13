using Grasshopper.Kernel.Types;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class FontAttributesGoo : GH_Goo<FontAttributes> {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla font attributes";

        public override string TypeName => typeof(FontAttributes).ToShortString();

        public FontAttributesGoo() {
        }

        public FontAttributesGoo(FontAttributes attr)
            : base(attr) {
        }
        public override IGH_Goo Duplicate() {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source) {
            if(source is FontAttributes) {
                Value=source as FontAttributes;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString() {
            if(Value==null)
                return "No value";
            //I'm not sure what this is for
            return $"{Value.Name}";
        }
    }
}
