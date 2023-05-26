using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateADrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateADrawing;

        private int _defaultInt = -1;
        public CreateADrawingComponent() : base(ComponentInfos.CreateADrawingComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Assembly", "A", "Assembly", GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);

            pManager.AddIntegerParameter("Sheet number", "SheetNumber", "The sheet number of the drawing.", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Drawing, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic dynamicObject = null;
            var parameterSet = DA.GetData("Assembly", ref dynamicObject);
            if (!parameterSet)
                return;

            var assembly = DynamicModelObjectConverter.GetAssembly(dynamicObject);
            if (assembly == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided input could not be converted to Assembly");
                return;
            }

            var attributesFileName = string.Empty;
            DA.GetData(ParamInfos.Attributes.Name, ref attributesFileName);
            if (string.IsNullOrEmpty(attributesFileName))
                attributesFileName = "standard";

            var sheetNumber = _defaultInt;
            DA.GetData("Sheet number", ref sheetNumber);

            var identifier = assembly.Identifier;

            AssemblyDrawing createdDrawing = sheetNumber == _defaultInt ?
                new AssemblyDrawing(identifier, attributesFileName) :
                new AssemblyDrawing(identifier, sheetNumber, attributesFileName);

            if (createdDrawing != null)
            {
                createdDrawing.Insert();
                DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(createdDrawing));
            }
        }
    }
}
