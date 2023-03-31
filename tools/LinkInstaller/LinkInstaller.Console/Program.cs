using LinkInstaller.Core;
using LinkInstaller.Validators;
using System.IO;

namespace LinkInstaller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!new ArgumentsValidator(args).IsValid())
                return;

            if (!new RhinoProcessValidator().IsValid())
                return;

            var settings = Settings.FromArguments(args);

            if (!new DirectoriesValidator(settings).IsValid())
                return;

            UnblockFiles(settings.PathToTeklaVerSourceDir);

            new GhaFileMover(settings.PathToTeklaVerSourceDir, settings.DestinationGhaPath)
                .MoveFiles();

            new RhinoRunner(settings.RhinoPath)
                .RunWithGrasshopper();
        }

        private static void UnblockFiles(string sourcePath)
        {
            foreach (var file in Directory.EnumerateFiles(sourcePath, "*.gha"))
            {
                new FileUnblocker()
                    .Unblock(file);
            }
        }
    }
}
