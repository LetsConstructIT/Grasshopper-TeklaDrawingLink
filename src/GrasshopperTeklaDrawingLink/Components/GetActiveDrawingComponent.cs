using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;

namespace GTDrawingLink.Components
{
    public class GetActiveDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.ActiveDrawing;

        public GetActiveDrawingComponent() : base(ComponentInfos.GetActiveDrawingComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanTrigger, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Drawing, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(ParamInfos.BooleanTrigger.Name, ref trigger);

            if (trigger)
            {
                var drawing = DrawingInteractor.GetActiveDrawing();

                DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(drawing));
            }
        }
    }
}
