using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class CircleAttributesParam : GH_PersistentParam<GH_Goo<Circle.CircleAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public CircleAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public CircleAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override GH_Goo<Circle.CircleAttributes> InstantiateT()
        {
            return new CircleAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<Circle.CircleAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<Circle.CircleAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
