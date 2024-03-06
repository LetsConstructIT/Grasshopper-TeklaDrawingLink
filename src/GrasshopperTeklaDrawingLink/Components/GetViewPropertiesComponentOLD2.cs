using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    [Obsolete]
    public class GetViewPropertiesComponentOLD2 : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.ViewProperties;

        public GetViewPropertiesComponentOLD2() : base(ComponentInfos.GetViewPropertiesComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.ViewType, GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the provided view", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            DA.SetData(ParamInfos.ViewType.Name, view.ViewType.ToString());
            DA.SetData("Name", view.Name);
        }
    }
}
