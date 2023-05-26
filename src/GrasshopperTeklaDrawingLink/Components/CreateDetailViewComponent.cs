using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDetailViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DetailView;
        private int _defaultRadius = 500;

        public CreateDetailViewComponent()
            : base(ComponentInfos.CreateDetailViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter("Center point", "CP", "Center point of detail", GH_ParamAccess.item);
            pManager.AddPointParameter("Label point", "LP", "Label location", GH_ParamAccess.item);
            pManager.AddPointParameter("Insertion point", "IP", "Detail view insertion point", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Radius", "R", "Detail range", GH_ParamAccess.item, _defaultRadius);
            pManager.AddTextParameter("View attributes", "VA", "View attributes file name", GH_ParamAccess.item, "standard");
            pManager.AddTextParameter("Mark attributes", "MA", "Detail mark attributes file name", GH_ParamAccess.item, "standard");
            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            AddGenericParameter(pManager, ParamInfos.Mark, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            Rhino.Geometry.Point3d centerPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Center point", ref centerPoint))
                return;

            var radius = 0;
            if (!DA.GetData("Radius", ref radius) || radius == 0)
                return;

            Rhino.Geometry.Point3d labelPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Label point", ref labelPoint))
                return;

            Rhino.Geometry.Point3d insertionPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Insertion point", ref insertionPoint))
                return;

            var viewAttributesFileName = string.Empty;
            DA.GetData("View attributes", ref viewAttributesFileName);
            var viewAttributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(viewAttributesFileName))
                viewAttributes.LoadAttributes(viewAttributesFileName);

            var scale = 0;
            DA.GetData(ParamInfos.Scale.Name, ref scale);
            if (scale != 0)
                viewAttributes.Scale = scale;

            var markAttributesFileName = string.Empty;
            DA.GetData("Mark attributes", ref markAttributesFileName);
            var markAttributes = new DetailMark.DetailMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            var viewName = string.Empty;
            DA.GetData(ParamInfos.Name.Name, ref viewName);
            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            var boundaryPoint = centerPoint + new Rhino.Geometry.Point3d(radius, 0, 0);

            View createdView = null;
            DetailMark createdMark = null;
            View.CreateDetailView(
                view,
                centerPoint.ToTeklaPoint(),
                boundaryPoint.ToTeklaPoint(),
                labelPoint.ToTeklaPoint(),
                insertionPoint.ToTeklaPoint(),
                viewAttributes,
                markAttributes,
                out createdView,
                out createdMark);

            if (createdView != null)
            {
                DA.SetData(ParamInfos.View.Name, new TeklaDatabaseObjectGoo(createdView));
                DA.SetData(ParamInfos.Mark.Name, createdMark);
            }
        }
    }
}
