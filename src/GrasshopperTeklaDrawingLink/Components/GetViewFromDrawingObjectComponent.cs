using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetViewFromDrawingObjectComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.GetViewFromDrawingObject;

        public GetViewFromDrawingObjectComponent() : base(ComponentInfos.GetViewFromDrawingObjectComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.ViewBase, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawingObject = DA.GetGooValue<DatabaseObject>(ComponentInfos.DrawingObjectParam) as DrawingObject;
            if (drawingObject == null)
                return;

            var viewBase = drawingObject.GetView();
            DA.SetData(ParamInfos.ViewBase.Name, new TeklaDatabaseObjectGoo(viewBase));
        }
    }
}
