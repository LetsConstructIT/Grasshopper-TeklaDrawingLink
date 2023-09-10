using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingSourceObjectComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingSourceObject;

        private string _modelObjectName = "ModelObject";
        public GetDrawingSourceObjectComponent() : base(ComponentInfos.GetDrawingSourceObjectComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(_modelObjectName, "MO", "Tekla model object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
                return;

            if (drawing == null)
                return;

            Identifier identifier = null;
            if (drawing is CastUnitDrawing)
                identifier = (drawing as CastUnitDrawing).CastUnitIdentifier;
            else if (drawing is AssemblyDrawing)
                identifier = (drawing as AssemblyDrawing).AssemblyIdentifier;
            else if (drawing is SinglePartDrawing)
                identifier = (drawing as SinglePartDrawing).PartIdentifier;

            if (identifier == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Not supported type of drawing");
                return;
            }

            DA.SetData(_modelObjectName, new ModelObjectGoo(ModelInteractor.GetModelObject(identifier)));
        }
    }
}
