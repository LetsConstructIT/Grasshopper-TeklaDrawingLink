using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetRelatedViewsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetRelatedViews;

        public GetRelatedViewsComponent() : base(ComponentInfos.GetRelatedViewsComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            var detailsDescription = new GH_InstanceDescription("Details", "D", "Details related to view", "", "");
            AddTeklaDbObjectParameter(pManager, detailsDescription, GH_ParamAccess.list);

            var sectionsDescription = new GH_InstanceDescription("Sections", "S", "Sections related to view", "", "");
            AddTeklaDbObjectParameter(pManager, sectionsDescription, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            var relatedViews = new RelatedViewsSearcher(view)
                .GetRelatedViews();

            var details = relatedViews.details;
            var sections = relatedViews.sections;

            DA.SetDataList("Details", details.Select(v => new TeklaDatabaseObjectGoo(v)));
            DA.SetDataList("Sections", sections.Select(v => new TeklaDatabaseObjectGoo(v)));
        }
    }
}
