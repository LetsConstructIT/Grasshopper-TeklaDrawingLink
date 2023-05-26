using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Goo<DatabaseObject>> list = new List<GH_Goo<DatabaseObject>>();
            if (!DA.GetDataList(ParamInfos.TeklaDatabaseObject.Name, list) || !list.Any())
            {
                return;
            }

            DA.SetDataList(ParamInfos.TeklaDatabaseObject.Name, list);
        }
    }
}
