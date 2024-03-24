using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;

namespace GTDrawingLink.Components.Drawings
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
            AddBooleanParameter(pManager, ParamInfos.BooleanToogle, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(0, ref trigger);

            if (trigger)
            {
                var drawing = DrawingInteractor.GetActiveDrawing();
                if (drawing is null)
                    DA.SetData(0, null);
                else
                    DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(drawing));
            }
        }
    }
}
