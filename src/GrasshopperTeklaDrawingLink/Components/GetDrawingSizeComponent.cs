using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingSizeComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingSize;

        public GetDrawingSizeComponent() : base(ComponentInfos.GetDrawingSizeComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Drawing, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Width", "W", "Drawing width", GH_ParamAccess.list);
            pManager.AddNumberParameter("Height", "H", "Drawing height", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
                return;

            var sheetSize = drawing.Layout.SheetSize;
            DA.SetData("Width", sheetSize.Width);
            DA.SetData("Height", sheetSize.Height);
        }
    }
}
