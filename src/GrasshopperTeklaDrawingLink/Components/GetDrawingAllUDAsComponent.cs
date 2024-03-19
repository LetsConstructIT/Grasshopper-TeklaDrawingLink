using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingAllUDAsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetAllUDAs;

        public GetDrawingAllUDAsComponent() : base(ComponentInfos.GetDrawingAllUDAsComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(ParamInfos.UDAsOutput.Name, ParamInfos.UDAsOutput.NickName, ParamInfos.UDAsOutput.Description, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var databaseObjects = DA.GetGooListValue<DatabaseObject>(ParamInfos.TeklaDatabaseObject);
            if (databaseObjects == null || !databaseObjects.Any())
                return;

            var attributes = new List<Attributes>();
            foreach (DatabaseObject modelObject in databaseObjects)
            {
                var localAttributes = AttributesIO.GetAll(modelObject);
                attributes.AddRange(localAttributes);
            }

            DA.SetDataList(ParamInfos.UDAsOutput.Name, attributes);
        }
    }
}
