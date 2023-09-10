using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class GetCOGComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetCOG;

        public GetCOGComponent() : base(ComponentInfos.GetCOGComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaGravityObjectParam(ParamInfos.GravityObject, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("COG", "G", "Center of gravity", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var gravityObject = DA.GetGooValue<ModelObject>(ParamInfos.GravityObject);
            if (gravityObject == null)
                return;

            double x = 0, y = 0, z = 0;
            gravityObject.GetReportProperty("COG_X", ref x);
            gravityObject.GetReportProperty("COG_Y", ref y);
            gravityObject.GetReportProperty("COG_Z", ref z);

            var point = new Rhino.Geometry.Point3d(x, y, z);
            DA.SetData("COG", point);
        }
    }
}
