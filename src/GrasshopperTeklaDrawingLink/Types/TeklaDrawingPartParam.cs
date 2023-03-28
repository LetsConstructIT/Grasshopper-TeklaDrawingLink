using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingPartParam : TeklaDrawingObjectFloatingParam
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DrawingPart;

        public TeklaDrawingPartParam() : base(ComponentInfos.DrawingPartParam, typeof(Part))
        {
        }
    }
}
