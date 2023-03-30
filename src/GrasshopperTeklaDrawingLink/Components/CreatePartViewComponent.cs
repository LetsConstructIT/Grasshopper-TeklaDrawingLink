using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreatePartViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.PartView;

        public CreatePartViewComponent() : base(ComponentInfos.CreatePartViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingParam(ParamInfos.Drawing, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.ViewType, GH_ParamAccess.item);
            pManager.AddPointParameter("Point", "P", "View insertion point", GH_ParamAccess.item);

            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingViewParam(ParamInfos.View, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<Drawing>(ParamInfos.Drawing);
            if (drawing == null)
                return;

            var viewType = string.Empty;
            var parameterSet = DA.GetData(ParamInfos.ViewType.Name, ref viewType);
            if (!parameterSet)
                return;

            Rhino.Geometry.Point3d insertionPoint = new Rhino.Geometry.Point3d();
            parameterSet = DA.GetData("Point", ref insertionPoint);
            if (!parameterSet)
                return;

            var attributesFileName = string.Empty;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFileName);

            var teklaPoint = insertionPoint.ToTeklaPoint();
            var attributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(attributesFileName))
                attributes.LoadAttributes(attributesFileName);

            var scale = 0;
            DA.GetData(ParamInfos.Scale.Name, ref scale);
            if (scale != 0)
                attributes.Scale = scale;

            View createdView = null;
            switch (viewType.ToUpper())
            {
                case "FRONT":
                    View.CreateFrontView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "TOP":
                    View.CreateTopView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "BOTTOM":
                    View.CreateBottomView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "BACK":
                    View.CreateBackView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "3D":
                    View.Create3dView(drawing, teklaPoint, attributes, out createdView);
                    break;
                default:
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You can only use Front | Top | Bottom | Back or 3D as input view type");
                    break;
            }

            if (createdView != null)
            {
                var viewName = string.Empty;
                DA.GetData(ParamInfos.Name.Name, ref viewName);
                if (!string.IsNullOrEmpty(viewName))
                {
                    createdView.Name = viewName;
                    createdView.Modify();
                    DrawingInteractor.CommitChanges();
                }

                DA.SetData(ParamInfos.View.Name, new TeklaDrawingViewGoo(createdView));
            }
        }
    }
}
