using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class GetDrawingAllUDAsComponentOLD : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.GetAllUDAs;

        public GetDrawingAllUDAsComponentOLD() : base(ComponentInfos.GetDrawingAllUDAsComponent)
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

            var attributes = new List<Tools.Attributes>();
            foreach (DatabaseObject modelObject in databaseObjects)
            {
                var localAttributes = AttributesIO.GetAll(modelObject);
                attributes.AddRange(localAttributes);
            }

            DA.SetDataList(ParamInfos.UDAsOutput.Name, attributes);
        }
    }
}
