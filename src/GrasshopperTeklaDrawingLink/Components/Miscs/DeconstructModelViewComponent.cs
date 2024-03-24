using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;

namespace GTDrawingLink.Components.Miscs
{
    public class DeconstructModelViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.DeconstructModelView;

        public DeconstructModelViewComponent() : base(ComponentInfos.DeconstructModelViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.ViewCoordinateSystem, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.DisplayCoordinateSystem, GH_ParamAccess.item);
            AddBoxParameter(pManager, ParamInfos.ViewRestrictionBox, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var modelView = DA.GetGooValue<TeklaView>(ParamInfos.ModelView);
            if (modelView == null)
                return;

            DA.SetData(ParamInfos.Name.Name, modelView.Name);
            DA.SetData(ParamInfos.ViewCoordinateSystem.Name, modelView.ViewCoordinateSystem);
            DA.SetData(ParamInfos.DisplayCoordinateSystem.Name, modelView.DisplayCoordinateSystem);
            DA.SetData(ParamInfos.ViewRestrictionBox.Name, modelView.RestrictionBox);
        }
    }
}
