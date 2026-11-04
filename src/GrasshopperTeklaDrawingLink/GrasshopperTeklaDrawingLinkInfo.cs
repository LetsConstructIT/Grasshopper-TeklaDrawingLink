using System;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel;

namespace GTDrawingLink
{
    public class GrasshopperTeklaDrawingLinkInfo : GH_AssemblyInfo
    {
        // Computed lazily instead of in a static constructor: an exception thrown from a type
        // initializer becomes a TypeInitializationException and makes the whole type unusable,
        // which is fatal when the assembly is loaded through the Rhino 9 ParseGHA fallback path.
        private static readonly Lazy<string> _linkVersion = new Lazy<string>(GetLinkVersion);
        private static readonly Lazy<string> _tsVersion = new Lazy<string>(GetTeklaVersion);

        public static string LinkVersion => _linkVersion.Value;

        public static string TSVersion => _tsVersion.Value;

        public GrasshopperTeklaDrawingLinkInfo()
        {
            // Grasshopper creates this object while parsing the assembly. Registering the UI here as well
            // keeps the "Drawing Link" menu working if PriorityLoad is not called for this assembly
            // (e.g. when the loader falls back to GH_ComponentServer.ParseGHA). Registration is idempotent.
            GrasshopperUIModifier.EnsureRegistered();
        }

        public override Guid Id => new Guid("27FB0C1C-8E69-4D88-8554-FE71C452AF8F");

        public override string Name => $"Grasshopper-Tekla drawing Link {LinkVersion}";

        public override string Version => LinkVersion;

        public override string AuthorName => "LetsConstructIT";

        public override string Description => "Components to link to Tekla Structures drawing area";

        private static string GetLinkVersion()
        {
            try
            {
                var version = typeof(GrasshopperTeklaDrawingLinkInfo).Assembly.GetName().Version;
                return $"{version.Major}.{version.Minor}";
            }
            catch (Exception)
            {
                return "?";
            }
        }

        private static string GetTeklaVersion()
        {
            try
            {
                var teklaLibrariesVersion = typeof(GrasshopperTeklaDrawingLinkInfo).Assembly
                    .GetReferencedAssemblies()
                    .FirstOrDefault((AssemblyName a) => a.Name == "Tekla.Structures.Model")?
                    .Version;

                return teklaLibrariesVersion != null ? $"{teklaLibrariesVersion.Major}" : "?";
            }
            catch (Exception)
            {
                return "?";
            }
        }
    }
}
