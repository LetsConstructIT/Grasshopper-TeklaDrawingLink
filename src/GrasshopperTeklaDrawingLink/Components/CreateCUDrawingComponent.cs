using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateCUDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateCUDrawing;

        private int _defaultInt = -1;
        public CreateCUDrawingComponent() : base(ComponentInfos.CreateCUDrawingComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Cast Unit", "CU", "Cast unit (assembly)", GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);

            pManager.AddIntegerParameter("Creation type", "CreationType", "Defines the Cast Unit Drawing Creation type. 0 -> by position, 1-> by id", GH_ParamAccess.item);
            pManager[pManager.ParamCount - 1].Optional = true;

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
            var parameterSet = DA.GetData("Cast Unit", ref dynamicObject);
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

            var creationTypeInt = _defaultInt;
            DA.GetData("Creation type", ref creationTypeInt);
            var creationType = (CastUnitDrawing.CastUnitDrawingCreationType)creationTypeInt;

            var sheetNumber = _defaultInt;
            DA.GetData("Sheet number", ref sheetNumber);

            var identifier = assembly.Identifier;

            CastUnitDrawing createdDrawing = sheetNumber == _defaultInt ?
                new CastUnitDrawing(identifier, creationType, attributesFileName) :
                new CastUnitDrawing(identifier, creationType, sheetNumber, attributesFileName);

            if (createdDrawing != null)
            {
                createdDrawing.Insert();
                DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(createdDrawing));
            }
        }
    }
}
