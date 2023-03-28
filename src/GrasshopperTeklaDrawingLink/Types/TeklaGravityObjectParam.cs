using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Model;

namespace GTDrawingLink.Types
{
    public class TeklaGravityObjectParam : GH_Param<GH_Goo<ModelObject>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaGravityObjectParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TeklaGravityObjectParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<ModelObject> InstantiateT()
        {
            return new TeklaGravityObjectGoo();
        }
    }
}
