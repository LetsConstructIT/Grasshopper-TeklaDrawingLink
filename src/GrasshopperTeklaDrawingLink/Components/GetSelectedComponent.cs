using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;

namespace GTDrawingLink.Components
{
    public class GetSelectedComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.SelectedObjects;

        public GetSelectedComponent() : base(ComponentInfos.GetSelectedComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanTrigger, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingObjectParam(ComponentInfos.DrawingObjectParam, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(ParamInfos.BooleanTrigger.Name, ref trigger);

            if (!trigger)
                return;

            var selected = new List<TeklaDrawingObjectGoo>();

            var doe = DrawingInteractor.DrawingHandler
                .GetDrawingObjectSelector()
                .GetSelected();

            while (doe.MoveNext())
            {
                selected.Add(new TeklaDrawingObjectGoo(doe.Current));
            }

            DA.SetDataList(ComponentInfos.DrawingObjectParam.Name, selected);
        }
    }
}
