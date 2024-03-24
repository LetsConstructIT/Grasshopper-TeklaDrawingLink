using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Rhino.Geometry;

namespace GTDrawingLink.Components.Miscs
{
    public class ConstructModelViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ConstructModelView;

        public ConstructModelViewComponent() : base(ComponentInfos.ConstructModelViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.ViewCoordinateSystem, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.DisplayCoordinateSystem, GH_ParamAccess.item);
            AddBoxParameter(pManager, ParamInfos.ViewRestrictionBox, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var name = string.Empty;
            DA.GetData(ParamInfos.Name.Name, ref name);

            Plane viewCoord = new Plane();
            if (!DA.GetData(ParamInfos.ViewCoordinateSystem.Name, ref viewCoord))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "View coordinate system is missing");
                return;
            }

            Plane displayCoord = new Plane();
            if (!DA.GetData(ParamInfos.DisplayCoordinateSystem.Name, ref displayCoord))
            {
                displayCoord = viewCoord;
            }

            Box restrictionBox = new Box();
            if (!DA.GetData(ParamInfos.ViewRestrictionBox.Name, ref restrictionBox))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Restriction box is missing");
                return;
            }

            var teklaView = new TeklaView(name, viewCoord, displayCoord, restrictionBox);
            DA.SetData(ParamInfos.ModelView.Name, new TeklaViewGoo(teklaView));
        }
    }
}
