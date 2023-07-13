using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class TransformPointToViewCSComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.TransformPointToView;

        public TransformPointToViewCSComponent() : base(ComponentInfos.TransformPointToViewCS)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "P", "Point to transform", GH_ParamAccess.list);
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
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
                return;

            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            view.Select();

            var matrix = T3D.MatrixFactory.ToCoordinateSystem(view.DisplayCoordinateSystem);
            DA.SetDataList("Point", points.Select(p => matrix.Transform(p.ToTeklaPoint()).ToRhinoPoint()));
        }
    }
}
