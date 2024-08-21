using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Views
{
    public class RefreshViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.RefreshView;

        public RefreshViewComponent() : base(ComponentInfos.RefreshViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var databaseObjects = DA.GetGooListValue<DatabaseObject>(ParamInfos.View);

            var views = new List<View>();
            foreach (var item in databaseObjects)
            {
                if (item is View)
                    views.Add(item as View);
            }

            if (!views.HasItems())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided View is null");
                return;
            }

            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return;
            }

            var initialFilters = GetInitialFilters(views);

            SetDummyFilter(views);
            RestoreFilters(views, initialFilters);

            DA.SetDataList(ParamInfos.View.Name, views.Select(v => new TeklaDatabaseObjectGoo(v)));
        }

        private void SetDummyFilter(List<View> views)
        {
            foreach (var view in views)
                view.SetFilter(DummyFilter);

            DrawingInteractor.CommitChanges();
        }

        private void RestoreFilters(List<View> views, string[] initialFilters)
        {
            for (int i = 0; i < views.Count; i++)
            {
                views[i].SetFilter(initialFilters[i]);
            }

            DrawingInteractor.CommitChanges();
        }

        private string[] GetInitialFilters(List<View> views)
        {
            var initialFilters = new string[views.Count];
            for (int i = 0; i < views.Count; i++)
                initialFilters[i] = views[i].GetFilter();

            return initialFilters;
        }

        private static string DummyFilter
            => @"TITLE_OBJECT_GROUP 
{
    Version= 1.05 
    Count= 1 
    SECTION_OBJECT_GROUP 
    {
        0 
        1 
        co_object 
        proGUID 
        albl_Guid 
        == 
        albl_Equals 
        00000000-0000-0000-0000-000000000000 
        0 
        && 
        }
    }
";
    }
}
