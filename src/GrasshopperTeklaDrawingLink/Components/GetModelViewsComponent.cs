using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TSMUI = Tekla.Structures.Model.UI;

namespace GTDrawingLink.Components
{
    public class GetModelViewsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetModelViews;

        public GetModelViewsComponent() : base(ComponentInfos.GetModelViewsComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanToogle, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(ParamInfos.BooleanToogle.Name, ref trigger);

            if (!trigger)
                return;

            var teklaViews = new List<TeklaView>();
            foreach (var view in GetAllModelViews())
            {
                teklaViews.Add(new TeklaView(view));
            }
            DA.SetDataList(ParamInfos.ModelView.Name, teklaViews.Select(v => new TeklaViewGoo(v)));
        }

        private IEnumerable<TSMUI.View> GetAllModelViews()
        {
            var mve = TSMUI.ViewHandler.GetAllViews();
            while (mve.MoveNext())
            {
                yield return mve.Current;
            }
        }
    }
}
