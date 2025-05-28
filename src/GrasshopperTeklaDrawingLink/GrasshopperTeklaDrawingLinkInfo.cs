using System;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel;

namespace GTDrawingLink
{
    public class GrasshopperTeklaDrawingLinkInfo : GH_AssemblyInfo
    {
        public static string LinkVersion { get; private set; }

        public static string TSVersion { get; private set; }

        public override Guid Id => new Guid("27FB0C1C-8E69-4D88-8554-FE71C452AF8F");

        public override string Name => $"Grasshopper-Tekla drawing Link {LinkVersion}";

        public override string Version => LinkVersion;

        public override string AuthorName => "LetsConstructIT";

        public override string Description => "Components to link to Tekla Structures drawing area";

        static GrasshopperTeklaDrawingLinkInfo()
        {
            var assembly = typeof(GrasshopperTeklaDrawingLinkInfo).Assembly;
            var version = assembly.GetName().Version;
            LinkVersion = $"{version.Major}.{version.Minor}";

            var teklaLibrariesVersion = assembly.GetReferencedAssemblies().First((AssemblyName a) => a.Name == "Tekla.Structures.Model").Version;
            TSVersion = $"{teklaLibrariesVersion.Major}";
        }
    }
}
