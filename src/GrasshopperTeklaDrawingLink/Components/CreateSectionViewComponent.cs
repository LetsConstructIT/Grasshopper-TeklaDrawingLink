using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateSectionViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.SectionView;

        private int _defaultDepth = 500;

        public CreateSectionViewComponent()
            : base(ComponentInfos.CreateSectionViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingViewParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter("Start point", "P1", "Start point of section line", GH_ParamAccess.item);
            pManager.AddPointParameter("End point", "P2", "End point of section line", GH_ParamAccess.item);
            pManager.AddPointParameter("Insertion point", "IP", "Detail view insertion point", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Depth up", "DU", "Section depth up", GH_ParamAccess.item, _defaultDepth);
            pManager.AddIntegerParameter("Depth down", "DD", "Sectino depth down", GH_ParamAccess.item, _defaultDepth);
            pManager.AddTextParameter("View attributes", "VA", "View attributes file name", GH_ParamAccess.item, "standard");
            pManager.AddTextParameter("Mark attributes", "MA", "Detail mark attributes file name", GH_ParamAccess.item, "standard");
            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingViewParam(ParamInfos.View, GH_ParamAccess.item));
            AddGenericParameter(pManager, ParamInfos.Mark, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            View view = null;
            var parameterSet = DA.GetData(ParamInfos.View.Name, ref view);
            if (!parameterSet)
                return;

            Rhino.Geometry.Point3d startPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Start point", ref startPoint))
                return;

            Rhino.Geometry.Point3d endPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("End point", ref endPoint))
                return;

            Rhino.Geometry.Point3d insertionPoint = new Rhino.Geometry.Point3d();
            parameterSet = DA.GetData("Insertion point", ref insertionPoint);
            if (!parameterSet)
                return;

            var depthUp = 0;
            if (!DA.GetData("Depth up", ref depthUp) || depthUp == 0)
                return;

            var depthDown = 0;
            if (!DA.GetData("Depth down", ref depthDown) || depthDown == 0)
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
            var markAttributes = new SectionMarkBase.SectionMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            var viewName = string.Empty;
            DA.GetData(ParamInfos.Name.Name, ref viewName);
            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            View createdView = null;
            SectionMark createdMark = null;
            View.CreateSectionView(
                view,
                startPoint.ToTeklaPoint(),
                endPoint.ToTeklaPoint(),
                insertionPoint.ToTeklaPoint(),
                depthUp,
                depthDown,
                viewAttributes,
                markAttributes,
                out createdView,
                out createdMark);

            if (createdView != null)
            {
                if (!string.IsNullOrEmpty(viewName))
                {
                    createdMark.Attributes.MarkName = viewName;
                    createdMark.Modify();
                    DrawingInteractor.CommitChanges();
                }

                DA.SetData(ParamInfos.View.Name, new TeklaDrawingViewBaseGoo(createdView));
                DA.SetData(ParamInfos.Mark.Name, createdMark);
            }
        }
    }
}
