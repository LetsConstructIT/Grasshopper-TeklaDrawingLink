using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class MoveViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.MoveView;

        public MoveViewComponent() : base(ComponentInfos.MoveViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
            pManager.AddVectorParameter("Vector", "Movement Vector", "Vector of movement", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            Rhino.Geometry.Vector3d vector = new Rhino.Geometry.Vector3d();
            var parameterSet = DA.GetData("Vector", ref vector);
            if (!parameterSet)
                return;

            var teklaVector = vector.ToTekla();

            view.Select();
            view.Origin += teklaVector;
            view.Modify();
            DrawingInteractor.CommitChanges();

            DA.SetData(ParamInfos.View.Name, new TeklaDatabaseObjectGoo(view));
        }
    }
}
