using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class CreateGADrawingComponent : CreateDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateGADrawing;

        public CreateGADrawingComponent() : base(ComponentInfos.CreateGADrawingComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanToggle, GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var trigger = false;
            if (!DA.GetData(ParamInfos.BooleanToggle.Name, ref trigger) || !trigger)
                return null;

            var viewName = string.Empty;
            if (!DA.GetData(ParamInfos.Name.Name, ref viewName))
                return null;

            var attributeFileName = string.Empty;
            if (!DA.GetData(ParamInfos.Attributes.Name, ref attributeFileName))
                return null;

            var createdDrawing = CreateGADrawing(
                viewName,
                attributeFileName);

            DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(createdDrawing));

            return new DatabaseObject[] { createdDrawing };
        }

        private GADrawing CreateGADrawing(string viewName, string attributesFileName)
        {
            if (string.IsNullOrEmpty(attributesFileName))
                attributesFileName = "standard";

            var drawing = new GADrawing(viewName, attributesFileName);
            drawing.Insert();

            return drawing;
        }
    }
}
