using System;
using System.Drawing;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Types
{
	public class TeklaDrawingObjectFloatingParam : TeklaDrawingObjectParam
	{

		public TeklaDrawingObjectFloatingParam()
			: this(ComponentInfos.DrawingObjectParam)
		{
		}

		public TeklaDrawingObjectFloatingParam(GH_InstanceDescription tag, params Type[] types)
			: base(tag, types)
		{
			_isFloatingParam = true;
		}
	}
}
