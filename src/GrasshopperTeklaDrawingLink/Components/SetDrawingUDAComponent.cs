using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class SetDrawingUDAComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.SetUDA;

        public SetDrawingUDAComponent() : base(ComponentInfos.SetDrawingUDAComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
            AddTextParameter(pManager, ParamInfos.UDAInput, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var databaseObjects = DA.GetGooListValue<DatabaseObject>(ParamInfos.TeklaDatabaseObject);
            if (databaseObjects == null || !databaseObjects.Any())
                return;

            var udas = new List<string>();
            DA.GetDataList(ParamInfos.UDAInput.Name, udas);
            if (udas == null || !udas.Any())
                return;

            List<Attributes> attributes = new List<Attributes>();
            foreach (string uda in udas)
            {
                Attributes item = null;
                if (!string.IsNullOrWhiteSpace(uda))
                    item = Tools.Attributes.Parse(uda);

                attributes.Add(item);
            }

            for (int i = 0; i < databaseObjects.Count; i++)
            {
                var databaseObject = databaseObjects[i];
                Attributes uDAs = attributes[Math.Min(i, attributes.Count - 1)];
                AttributesIO.SetUDAs(databaseObject, uDAs);
            }

            DA.SetDataList(ParamInfos.TeklaDatabaseObject.Name, databaseObjects);
        }
    }
}
