using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingPartParam : TeklaDatabaseObjectFloatingParam
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.DrawingPart;

        public TeklaDrawingPartParam() : base(ComponentInfos.DrawingPartParam, typeof(Part))
        {
        }

        public TeklaDrawingPartParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, typeof(Part))
        {
            Access = access;
        }
    }
}
