using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolylineAttributesParam : GH_PersistentParam<GH_Goo<Polyline.PolylineAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolylineAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolylineAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Polyline.PolylineAttributes> InstantiateT()
        {
            return new PolylineAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Polyline.PolylineAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Polyline.PolylineAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
