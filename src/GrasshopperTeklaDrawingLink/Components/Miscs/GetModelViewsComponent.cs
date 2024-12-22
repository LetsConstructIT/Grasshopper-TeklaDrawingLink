using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TSMUI = Tekla.Structures.Model.UI;

namespace GTDrawingLink.Components.Miscs
{
    public class GetModelViewsComponent : TeklaComponentBase
    {
        private GetViewMode _mode = GetViewMode.All;
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetModelViews;

        public GetModelViewsComponent() : base(ComponentInfos.GetModelViewsComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.AllModelViews.Name, GetViewModeAllMenuItem_Clicked, true, _mode == GetViewMode.All).ToolTipText = ParamInfos.AllModelViews.Description;
            Menu_AppendItem(menu, ParamInfos.VisibleModelViews.Name, GetViewModeVisibleMenuItem_Clicked, true, _mode == GetViewMode.Visible).ToolTipText = ParamInfos.VisibleModelViews.Description;
            Menu_AppendSeparator(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

        private void GetViewModeAllMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GetViewMode.All;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void GetViewModeVisibleMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GetViewMode.Visible;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case GetViewMode.All:
                    Message = "All model views";
                    break;
                case GetViewMode.Visible:
                    Message = "Only visible model views";
                    break;
                default:
                    Message = "";
                    break;
            }
        }

        private readonly string _itemName = "GetViewModeValue";
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(_itemName, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(_itemName, ref serializedInt);
            _mode = (GetViewMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanToggle, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var trigger = false;
            DA.GetData(ParamInfos.BooleanToggle.Name, ref trigger);

            if (!trigger)
                return;

            var teklaViews = new List<TeklaView>();
            foreach (var view in OrderByName(GetViews()))
            {
                teklaViews.Add(new TeklaView(view));
            }
            DA.SetDataList(ParamInfos.ModelView.Name, teklaViews.Select(v => new TeklaViewGoo(v)));
        }

        private IEnumerable<TSMUI.View> GetViews()
        {
            switch (_mode)
            {
                case GetViewMode.Visible:
                    return GetVisibleViews();
                default:
                    return GetAllModelViews();
            }
        }

        private IEnumerable<TSMUI.View> GetAllModelViews()
        {
            var mve = TSMUI.ViewHandler.GetAllViews();
            while (mve.MoveNext())
            {
                yield return mve.Current;
            }
        }

        private IEnumerable<TSMUI.View> GetVisibleViews()
        {
            var mve = TSMUI.ViewHandler.GetVisibleViews();
            while (mve.MoveNext())
            {
                yield return mve.Current;
            }
        }
        private IOrderedEnumerable<TSMUI.View> OrderByName(IEnumerable<TSMUI.View> views)
        {
            return views.OrderBy(v => v.Name, new NaturalStringComparer());
        }

        enum GetViewMode
        {
            All,
            Visible
        }
    }
}
