using Grasshopper.Kernel;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Types
{
    public class TeklaDatabaseObjectFloatingParam : TeklaDatabaseObjectParam
	{

		public TeklaDatabaseObjectFloatingParam()
			: this(ComponentInfos.DrawingObjectParam)
		{
		}

		public TeklaDatabaseObjectFloatingParam(GH_InstanceDescription tag)
			: base(tag)
		{
			_isFloatingParam = true;
		}
	}
}
