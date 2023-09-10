using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetViewsAtDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ViewsAtDrawing;

        public GetViewsAtDrawingComponent() : base(ComponentInfos.GetViewsAtDrawingComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
                return;

            var viewsAtDrawing = new List<TeklaDatabaseObjectGoo>();

            var viewEnumerator = drawing.GetSheet().GetViews();
            while (viewEnumerator.MoveNext())
                viewsAtDrawing.Add(new TeklaDatabaseObjectGoo(viewEnumerator.Current as View));

            DA.SetDataList(ParamInfos.View.Name, viewsAtDrawing);
        }
    }
}
