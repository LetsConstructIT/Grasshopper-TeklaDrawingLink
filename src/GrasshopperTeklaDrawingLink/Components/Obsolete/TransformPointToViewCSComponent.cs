using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Obsolete
{
    public class TransformPointToViewCSComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.TransformPointToView;

        public TransformPointToViewCSComponent() : base(ComponentInfos.TransformPointToViewCS)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point to transform", GH_ParamAccess.list);
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point after transformation", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Rhino.Geometry.Point3d> points = new List<Rhino.Geometry.Point3d>();
            var parameterSet = DA.GetDataList("Point", points);
            if (!parameterSet)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided Points are null");
                return;
            }

            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided View is null");
                return;
            }

            view.Select();

            var matrix = T3D.MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
            DA.SetDataList("Point", points.Select(p => matrix.Transform(p.ToTekla()).ToRhino()));
        }
    }
}
