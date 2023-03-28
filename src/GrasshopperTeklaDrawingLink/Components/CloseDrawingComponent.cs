using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CloseDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.septenary;
        protected override Bitmap Icon => Properties.Resources.CloseDrawing;

        public CloseDrawingComponent() : base(ComponentInfos.CloseDrawingComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingParam(ParamInfos.Drawing, GH_ParamAccess.item));
            pManager.AddBooleanParameter("Save", "S", "Should drawing be saved (true by default)", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Result", "R", "True when drawing was closed", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<Drawing>(ParamInfos.Drawing);
            if (drawing == null)
                return;

            var save = true;
            DA.GetData("Save", ref save);

            var result = DrawingInteractor.DrawingHandler.CloseActiveDrawing(save);
            DA.SetData("Result", result);
        }
    }
}
