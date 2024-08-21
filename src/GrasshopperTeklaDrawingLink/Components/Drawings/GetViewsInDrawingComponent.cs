using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class GetViewsInDrawingComponent : TeklaComponentBase
    {
        private QueryMode _mode = QueryMode.AllViews;

        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ViewsAtDrawing;

        public GetViewsInDrawingComponent() : base(ComponentInfos.GetViewsInDrawingComponent)
        {
            SetCustomMessage();
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
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided Drawing is null");
                return;
            }

            var viewsAtDrawing = new List<TeklaDatabaseObjectGoo>();

            var viewEnumerator = _mode == QueryMode.AllViews ?
                drawing.GetSheet().GetAllViews() :
                drawing.GetSheet().GetViews();

            while (viewEnumerator.MoveNext())
            {
                if (viewEnumerator.Current is TSD.View view)
                    viewsAtDrawing.Add(new TeklaDatabaseObjectGoo(view));
            }

            DA.SetDataList(ParamInfos.View.Name, viewsAtDrawing);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.AllViews.Name, AllViewsMenuItem_Clicked, true, _mode == QueryMode.AllViews).ToolTipText = ParamInfos.AllViews.Description;
            Menu_AppendItem(menu, ParamInfos.OnlyTopMostViews.Name, OnlyTopMostMenuItem_Clicked, true, _mode == QueryMode.OnlyTopMost).ToolTipText = ParamInfos.OnlyTopMostViews.Description;
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

        private void AllViewsMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = QueryMode.AllViews;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void OnlyTopMostMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = QueryMode.OnlyTopMost;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            Message = _mode switch
            {
                QueryMode.AllViews => "All views",
                QueryMode.OnlyTopMost => "Top most views",
                _ => "",
            };
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.AllObjects.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.AllObjects.Name, ref serializedInt);
            _mode = (QueryMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        enum QueryMode
        {
            AllViews,
            OnlyTopMost
        }
    }
}
