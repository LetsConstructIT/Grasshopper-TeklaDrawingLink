using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class ObjectMatchesToFilterComponentOLD : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.ObjectMatchesToFilter;

        public ObjectMatchesToFilterComponentOLD() : base(ComponentInfos.ObjectMatchesToFilterComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.ModelObject, GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.ObjectFilter, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.ObjectMatch, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic inputObject = null;
            if (!DA.GetData(ParamInfos.ModelObject.Name, ref inputObject))
                return;

            if (!(inputObject.Value is Tekla.Structures.Model.ModelObject modelObject))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided object is not Tekla ModelObject");
                return;
            }

            var filterName = string.Empty;
            DA.GetData(ParamInfos.ObjectFilter.Name, ref filterName);
            if (string.IsNullOrEmpty(filterName))
                return;

            var isMatch = Tekla.Structures.Model.Operations.Operation.ObjectMatchesToFilter(modelObject, filterName);

            DA.SetData(ParamInfos.ObjectMatch.Name, isMatch);
        }
    }
}
