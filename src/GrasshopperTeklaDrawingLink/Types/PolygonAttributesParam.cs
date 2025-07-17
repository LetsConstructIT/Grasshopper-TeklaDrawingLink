using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class PolygonAttributesParam : GH_PersistentParam<GH_Goo<Polygon.PolygonAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public PolygonAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public PolygonAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Polygon.PolygonAttributes> InstantiateT()
        {
            return new PolygonAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Polygon.PolygonAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Polygon.PolygonAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
