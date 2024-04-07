using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Extensions;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;
using System.Drawing;
using Rhino.Geometry;

namespace GTDrawingLink.Components.Views
{
    public class GetViewFrameGeometryComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ViewFrame;

        public GetViewFrameGeometryComponent() : base(ComponentInfos.GetViewFrameGeometryComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Upper left", "UL", "Upper left corner point", GH_ParamAccess.item);
            pManager.AddPointParameter("Upper right", "UR", "Upper right corner point", GH_ParamAccess.item);
            pManager.AddPointParameter("Lower left", "LL", "Lower left corner point", GH_ParamAccess.item);
            pManager.AddPointParameter("Lower right", "LR", "Lower right corner point", GH_ParamAccess.item);
            pManager.AddRectangleParameter("Rectangle", "R", "Rectangle representing view bounding box (blue box)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Frame width", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Frame height", GH_ParamAccess.item);
            pManager.AddPointParameter("Origin", "O", "View origin (drawing view coordinate system)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var viewBase = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (viewBase == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided View is null");
                return;
            }

            viewBase.Select();
            var lowerLeft = viewBase.Origin + viewBase.FrameOrigin;
            var lowerRight = lowerLeft + new T3D.Vector(1, 0, 0) * viewBase.Width;

            var upperLeft = lowerLeft + new T3D.Vector(0, 1, 0) * viewBase.Height;
            var upperRight = lowerRight + new T3D.Vector(0, 1, 0) * viewBase.Height;

            DA.SetData("Upper left", upperLeft.ToRhino());
            DA.SetData("Upper right", upperRight.ToRhino());
            DA.SetData("Lower left", lowerLeft.ToRhino());
            DA.SetData("Lower right", lowerRight.ToRhino());
            DA.SetData("Rectangle", new Rectangle3d(Plane.WorldXY, lowerLeft.ToRhino(), upperRight.ToRhino()));
            DA.SetData("Width", viewBase.Width);
            DA.SetData("Height", viewBase.Height);
            DA.SetData("Origin", viewBase.Origin.ToRhino());
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }
}
