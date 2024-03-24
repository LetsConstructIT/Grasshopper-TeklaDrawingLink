using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Parts
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
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetDataTree(0, out GH_Structure<GH_Goo<DatabaseObject>> tree))
                return;

            var drawingObjects = new List<DrawingObject>();
            foreach (var branch in tree.Branches)
            {
                foreach (var item in branch)
                {
                    if (item.Value is DrawingObject drawingObject)
                        drawingObjects.Add(drawingObject);
                }
            }

            DrawingInteractor.Highlight(drawingObjects);

            DA.SetDataTree(0, tree);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }
}
