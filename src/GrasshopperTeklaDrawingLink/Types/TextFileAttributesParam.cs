using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TextFileAttributesParam : GH_PersistentParam<GH_Goo<TextFile.TextFileAttributes>>
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

        protected override GH_Goo<TextFile.TextFileAttributes> InstantiateT()
        {
            return new TextFileAttributesGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<TextFile.TextFileAttributes> value)
            => GH_GetterResult.cancel;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<TextFile.TextFileAttributes>> values)
            => GH_GetterResult.cancel;
    }
}
