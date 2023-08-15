using Grasshopper.Kernel.Types;

using System;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types {
    public class ArrowAttributesGoo : GH_Goo<ArrowheadAttributes> {
        public override bool IsValid => true;

        public override string TypeDescription => "Tekla font attributes";

        public override string TypeName => typeof(ArrowheadAttributes).ToShortString();

        public ArrowAttributesGoo() {
        }

        public ArrowAttributesGoo(ArrowheadAttributes attr)
            : base(attr) {
        }
        public override IGH_Goo Duplicate() {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source) {
            if(source is ArrowheadAttributes) {
                Value = source as ArrowheadAttributes;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString() {
            if(Value == null)
                return "No value";
            return $"{Value.Head} + {Value.ArrowPosition} + {Value.Width} + {Value.Height}";
        }
    }
}
