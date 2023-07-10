using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDrawingLink.Components
{
    public class DeconstructDatabaseObjectComponentAttributes : GH_ComponentAttributes
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
					DeconstructDatabaseObjectComponentBase deconstructModelObjectBaseComponent = base.Owner as DeconstructDatabaseObjectComponentBase;
					if (value)
					{
						deconstructModelObjectBaseComponent.HighlightObjects();
					}
					else
					{
						deconstructModelObjectBaseComponent.UnHighlightObjects();
					}
				}
				base.Selected = value;
			}
		}

		public DeconstructDatabaseObjectComponentAttributes(DeconstructDatabaseObjectComponentBase owner)
			: base(owner)
		{
		}
	}
}
