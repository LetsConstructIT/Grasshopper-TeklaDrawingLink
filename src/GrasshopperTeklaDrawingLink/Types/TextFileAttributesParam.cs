using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;

namespace GTDrawingLink.Types
{
    public class TextFileAttributesParam : GH_PersistentParam<TextFileAttributesGoo>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TextFileAttributesParam(GH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TextFileAttributesParam(GH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag)
        {
            Access = access;
        }

        protected override TextFileAttributesGoo InstantiateT()
        {
            return new TextFileAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref TextFileAttributesGoo value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<TextFileAttributesGoo> values)
            => GH_GetterResult.cancel;
    }
}
