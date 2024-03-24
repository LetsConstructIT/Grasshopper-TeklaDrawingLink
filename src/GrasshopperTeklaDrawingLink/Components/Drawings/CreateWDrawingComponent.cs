using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components.Drawings
{
    public class CreateWDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateWDrawing;

        private int _defaultInt = -1;
        public CreateWDrawingComponent() : base(ComponentInfos.CreateWDrawingComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Part", "P", "Model part", GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);

            pManager.AddIntegerParameter("Sheet number", "SheetNumber", "The sheet number of the drawing.", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic dynamicObject = null;
            var parameterSet = DA.GetData("Part", ref dynamicObject);
            if (!parameterSet)
                return;

            var part = dynamicObject.Value as TSM.Part;
            if (part == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided input could not be converted to Model Object");
                return;
            }

            var attributesFileName = string.Empty;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFileName);
            if (string.IsNullOrEmpty(attributesFileName))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Drawing Attributes filename is missing");
                return;
            }

            var sheetNumber = _defaultInt;
            DA.GetData("Sheet number", ref sheetNumber);

            var identifier = part.Identifier;

            SinglePartDrawing createdDrawing = sheetNumber == _defaultInt ?
                new SinglePartDrawing(identifier, attributesFileName) :
                new SinglePartDrawing(identifier, sheetNumber, attributesFileName);

            if (createdDrawing != null)
            {
                createdDrawing.Insert();
                DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(createdDrawing));
            }
        }
    }
}
