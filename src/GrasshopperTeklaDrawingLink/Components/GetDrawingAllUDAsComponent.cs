using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
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
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
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

            List<Attributes> attributes = new List<Attributes>();
            foreach (DatabaseObject modelObject in databaseObjects)
            {
                Dictionary<string, string> stringDict;
                if (modelObject.GetStringUserProperties(out stringDict) && stringDict.Count > 0)
                    attributes.Add(Tools.Attributes.Parse(stringDict));

                Dictionary<string, int> intDict;
                if (modelObject.GetIntegerUserProperties(out intDict) && intDict.Count > 0)
                    attributes.Add(Tools.Attributes.Parse(intDict));

                Dictionary<string, double> dblDict;
                if (modelObject.GetDoubleUserProperties(out dblDict) && dblDict.Count > 0)
                    attributes.Add(Tools.Attributes.Parse(dblDict));
            }

            DA.SetDataList(ParamInfos.UDAsOutput.Name, attributes);
        }
    }
}
