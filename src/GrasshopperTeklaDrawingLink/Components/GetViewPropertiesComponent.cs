using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetViewPropertiesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.ViewProperties;

        public GetViewPropertiesComponent() : base(ComponentInfos.GetViewPropertiesComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.ViewType, GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the provided view", GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.ViewPlane, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;
            
            DA.SetData(ParamInfos.ViewType.Name, view.ViewType.ToString());
            DA.SetData("Name", view.Name);
            DA.SetData(ParamInfos.ViewPlane.Name, GetPlane(view));
        }

        private GH_Plane GetPlane(View view)
        {
            var coordSystem = view.DisplayCoordinateSystem;

            var geoPlane = new Rhino.Geometry.Plane(
                coordSystem.Origin.ToRhinoPoint(),
                coordSystem.AxisX.ToRhinoVector(),
                coordSystem.AxisY.ToRhinoVector());

            return new GH_Plane(geoPlane);
        }
    }
}
