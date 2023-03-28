using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetSelectedDrawingsOnListComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.SelectedDrawingsFromList;

        public GetSelectedDrawingsOnListComponent() : base(ComponentInfos.GetSelectedDrawingsOnListComponent)
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

            if (trigger)
            {
                var selectedDrawings = new List<TeklaDrawingGoo>();

                var drawingEnumerator = DrawingInteractor.DrawingHandler.GetDrawingSelector().GetSelected();
                while (drawingEnumerator.MoveNext())
                    selectedDrawings.Add(new TeklaDrawingGoo(drawingEnumerator.Current));

                DA.SetDataList(ParamInfos.Drawing.Name, selectedDrawings);
            }
        }
    }
}
