using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GTDrawingLink.Components
{
    public class GetExtremePointsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetExtremes;

        public GetExtremePointsComponent() : base(ComponentInfos.GetExtremePointsComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter(ParamInfos.Points.Name, ParamInfos.Points.NickName, ParamInfos.Points.Description, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddPointParameter(pManager, ParamInfos.HorizontalExtremes, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.VerticalExtremes, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var points = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.Points.Name, points) || !points.Any())
                return;

            var (horizontalExtremes, verticalExtremes) = GetExtremes(points);

            DA.SetDataList(ParamInfos.HorizontalExtremes.Name, horizontalExtremes);
            DA.SetDataList(ParamInfos.VerticalExtremes.Name, verticalExtremes);
        }

        private (List<Point3d> horizontalExtremes, List<Point3d> verticalExtremes) GetExtremes(List<Point3d> points)
        {
            var orderedByX = points.OrderBy(p => p.X).ThenBy(p => p.Y);
            var orderedByY = points.OrderBy(p => p.Y).ThenBy(p => p.X);

            var horizontalExtremes = new List<Point3d>
            {
                orderedByX.First(),
                orderedByX.Last()
            };

            var verticalExtremes = new List<Point3d>
            {
                orderedByY.First(),
                orderedByY.Last()
            };

            return (horizontalExtremes, verticalExtremes);
        }
    }
}
