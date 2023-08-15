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
    public class GetDrawingUDAValueComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetUDA;

        public GetDrawingUDAValueComponent() : base(ComponentInfos.GetDrawingUDAValueComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list);
            AddTextParameter(pManager, ParamInfos.UDAName, GH_ParamAccess.item, true);
            pManager.AddParameter(new EnumParam<AttributesIO.AttributeTypeEnum>(ParamInfos.UDAType, GH_ParamAccess.item, 0));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(ParamInfos.UDAValue.Name, ParamInfos.UDAValue.NickName, ParamInfos.UDAValue.Description, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var databaseObjects = DA.GetGooListValue<DatabaseObject>(ParamInfos.TeklaDatabaseObject);
            if (databaseObjects == null || !databaseObjects.Any())
                return;

            string udaName = "";
            if (!DA.GetData(ParamInfos.UDAName.Name, ref udaName))
                return;

            object udaTypeInput = null;
            DA.GetData(ParamInfos.UDAType.Name, ref udaTypeInput);
            if (udaTypeInput == null)
                return;

            var udaType = EnumHelpers.ObjectToEnumValue<AttributesIO.AttributeTypeEnum>(udaTypeInput).Value;

            var udaValues = new List<object>();
            foreach (DatabaseObject databaseObject in databaseObjects)
            {
                object obj = AttributesIO.GetUDAValue(databaseObject, udaName, udaType);
                if (obj == null && databaseObject is Plugin)
                    obj = AttributesIO.GetComponentAttributeValue(databaseObject as Plugin, udaName, udaType);

                udaValues.Add(obj);
            }
            DA.SetDataList(ParamInfos.UDAValue.Name, udaValues);
        }
    }
}
