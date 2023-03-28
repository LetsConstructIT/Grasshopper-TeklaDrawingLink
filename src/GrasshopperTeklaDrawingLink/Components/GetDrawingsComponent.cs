using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.AllDrawings;

        public GetDrawingsComponent() : base(ComponentInfos.GetDrawingsComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanTrigger, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingParam(ParamInfos.Drawing, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(ParamInfos.BooleanTrigger.Name, ref trigger);

            if (!trigger)
                return;

            var drawings = new List<TeklaDrawingGoo>();
            var drawingEnumerator = DrawingInteractor.DrawingHandler.GetDrawings();
            drawingEnumerator.SelectInstances = false;
            while (drawingEnumerator.MoveNext())
            {
                drawings.Add(new TeklaDrawingGoo(drawingEnumerator.Current));
            }

            DA.SetDataList(ParamInfos.Drawing.Name, drawings);
        }
    }
}
