using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Types
{
    public abstract class TeklaParamBase<T> : GH_PersistentParam<GH_Goo<T>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaParamBase(GH_InstanceDescription tag)
            : base(tag)
        {
        }
    }
}
