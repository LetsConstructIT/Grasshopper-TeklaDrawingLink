using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class BoltAttributesParam : GH_PersistentParam<GH_Goo<Bolt.BoltAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public BoltAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public BoltAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Bolt.BoltAttributes> InstantiateT()
        {
            return new BoltAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Bolt.BoltAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Bolt.BoltAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
