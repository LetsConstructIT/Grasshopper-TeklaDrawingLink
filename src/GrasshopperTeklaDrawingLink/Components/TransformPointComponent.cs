using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class TransformPointComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.TransformPoint;

        public TransformPointComponent() : base(ComponentInfos.TransformPointComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point to transform", GH_ParamAccess.item);
            pManager.AddParameter(new TeklaDrawingViewParam(ParamInfos.View, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point after transformation", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Point3d point = new Rhino.Geometry.Point3d();
            var parameterSet = DA.GetData("Point", ref point);
            if (!parameterSet)
                return;

            var view = DA.GetGooValue<View>(ParamInfos.View);
            if (view == null)
                return;

            var teklaPoint = point.ToTeklaPoint();

            view.Select();

            var matrix = T3D.MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
            var resultPoint = matrix.Transform(teklaPoint);

            DA.SetData("Point", new GH_Point(resultPoint.ToRhinoPoint()));
        }
    }
}
