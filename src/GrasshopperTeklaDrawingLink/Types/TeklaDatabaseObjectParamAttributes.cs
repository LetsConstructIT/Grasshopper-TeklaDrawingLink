using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDrawingLink.Types
{
    public class TeklaDatabaseObjectParamAttributes : GH_FloatingParamAttributes
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
                    TeklaDatabaseObjectParam teklaModelObjectParam = base.Owner as TeklaDatabaseObjectParam;
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

        public TeklaDatabaseObjectParamAttributes(TeklaDatabaseObjectParam owner)
            : base(owner)
        {

        }
    }
}
