using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class ConvertDrawingToModelObjectComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ConvertDrawingToModelObject;

        public ConvertDrawingToModelObjectComponent() : base(ComponentInfos.ConvertDrawingToModelObjectComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            var inputParam = new TeklaDatabaseObjectParam(ComponentInfos.DrawingPartParam, typeof(DrawingObject));
            pManager.AddParameter(inputParam);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model Object", "MO", "Tekla model object", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object input = null;
            DA.GetData(ComponentInfos.DrawingPartParam.Name, ref input);

            if (input is TeklaDatabaseObjectGoo)
            {
                var databaseObject = (input as TeklaDatabaseObjectGoo).Value;
                if (databaseObject is ModelObject drawingObject)
                {
                    var model = new TSM.Model();
                    var modelObject = model.SelectModelObject(drawingObject.ModelIdentifier);

                    DA.SetData("Model Object", modelObject);
                }
            }
        }
    }
}
