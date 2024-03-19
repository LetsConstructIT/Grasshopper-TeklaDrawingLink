using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class SelectDrawingObjectComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.SelectDrawingObject;
        public SelectDrawingObjectComponent() : base(ComponentInfos.SelectDrawingObjectComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawingObjects = DA.GetGooListValue<DatabaseObject>(ComponentInfos.DrawingObjectParam).Cast<DrawingObject>();
            if (drawingObjects == null)
                return;

            DrawingInteractor.Highlight(drawingObjects);

            DA.SetDataList(ComponentInfos.DrawingObjectParam.Name, drawingObjects.Select(d => new TeklaDatabaseObjectGoo(d)));
        }
    }
}
