using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateLevelMarkComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.LevelMark;

        public CreateLevelMarkComponent() : base(ComponentInfos.CreateLevelMarkComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingViewParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter("Insertion point", "IP", "Insertion point of the Level Mark", GH_ParamAccess.item);
            pManager.AddPointParameter("Base point", "BP", "Base point of the Level Mark", GH_ParamAccess.item);
            pManager.AddTextParameter("Mark attributes", "MA", "Level mark attributes file name", GH_ParamAccess.item, "standard");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.Mark, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<View>(ParamInfos.View);
            if (view == null)
                return;

            Rhino.Geometry.Point3d insertionPoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Insertion point", ref insertionPoint))
                return;

            Rhino.Geometry.Point3d basePoint = new Rhino.Geometry.Point3d();
            if (!DA.GetData("Base point", ref basePoint))
                return;

            var levelMarkAttributesFileName = string.Empty;
            DA.GetData("Mark attributes", ref levelMarkAttributesFileName);
            var levelMarkAttributes = new LevelMark.LevelMarkAttributes();
            if (!string.IsNullOrEmpty(levelMarkAttributesFileName))
                levelMarkAttributes.LoadAttributes(levelMarkAttributesFileName);

            var levelMark = new LevelMark(
                view, 
                insertionPoint.ToTeklaPoint(), 
                basePoint.ToTeklaPoint(), 
                levelMarkAttributes);

            levelMark.Insert();

            DrawingInteractor.CommitChanges();

            if (levelMark != null)
            {
                DA.SetData(ParamInfos.Mark.Name, levelMark);
            }
        }
    }
}
