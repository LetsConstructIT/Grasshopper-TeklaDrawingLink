using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingObjectParamAttributes : GH_FloatingParamAttributes
    {
        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                if (value != Selected)
                {
                    TeklaDrawingObjectParam teklaModelObjectParam = base.Owner as TeklaDrawingObjectParam;
                    if (value)
                    {
                        teklaModelObjectParam.HighlightObjects();
                    }
                    else
                    {
                        teklaModelObjectParam.UnHighlightObjects();
                    }
                }
                base.Selected = value;
            }
        }

        public TeklaDrawingObjectParamAttributes(TeklaDrawingObjectParam owner)
            : base(owner)
        {

        }
    }
}
