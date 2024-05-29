using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TextFileAttributesParam : GH_Param<GH_Goo<TextFile.TextFileAttributes>>
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TextFileAttributesParam(IGH_InstanceDescription tag)
            : base(tag)
        {
        }

        public TextFileAttributesParam(IGH_InstanceDescription tag, GH_ParamAccess access)
            : base(tag, access)
        {
        }

        protected override GH_Goo<TextFile.TextFileAttributes> InstantiateT()
        {
            return new TextFileAttributesGoo();
        }
    }

    
}
