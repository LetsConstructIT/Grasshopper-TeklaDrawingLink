using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetViewPropertiesComponentOLD : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ViewProperties;

        public GetViewPropertiesComponentOLD() : base(ComponentInfos.GetViewPropertiesComponent)
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
            AddPlaneParameter(pManager, ParamInfos.ViewCoordinateSystem, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.DisplayCoordinateSystem, GH_ParamAccess.item);
            AddBoxParameter(pManager, ParamInfos.ViewRestrictionBox, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;
            view.Select();

            DA.SetData(ParamInfos.ViewType.Name, view.ViewType.ToString());
            DA.SetData("Name", view.Name);
            DA.SetData(ParamInfos.ViewCoordinateSystem.Name, view.ViewCoordinateSystem.ToRhino());
            DA.SetData(ParamInfos.DisplayCoordinateSystem.Name, view.DisplayCoordinateSystem.ToRhino());
            DA.SetData(ParamInfos.ViewRestrictionBox.Name, GetRestrictionBox(view));
        }

        private GH_Box GetRestrictionBox(View view)
        {
            var box = new Rhino.Geometry.Box(
                view.ViewCoordinateSystem.ToRhino(),
                view.RestrictionBox.ToRhino());

            return new GH_Box(box);
        }
    }
}
