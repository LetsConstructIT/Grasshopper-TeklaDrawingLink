using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {

        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

        }
    }
}
