using LinkInstaller.Core;
using System;
using System.IO;
using System.Linq;

namespace LinkInstaller.Validators
{
    public class DirectoriesValidator : ConsoleValidator
    {
        private Settings _settings;

        public DirectoriesValidator(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public override bool IsValid()
        {
            var sourcePath = _settings.PathToTeklaVerSourceDir;
            if (!Directory.Exists(sourcePath))
            {
                ShowErrorToUser(new string[]
                {
                    $"Subdirectory for Tekla version {_settings.TeklaVersion} was not found",
                    $"in directory {_settings.SourceGhaPath}.",
                    "",
                    "Check its existence and modify startup Tekla macro."
                });
                return false;
            }

            if (!Directory.EnumerateFiles(sourcePath, "*.gha").Any())
            {
                ShowErrorToUser(new string[]
                {
                    $"Source directory {sourcePath} does not have .gha files.",
                    "Nothing to copy."
                });
                return false;
            }

            if (Directory.EnumerateFiles(sourcePath, "*.gha")
                .Where(f => Path.GetFileName(f).StartsWith("GrasshopperTeklaDrawingLink_"))
                .Count() > 1)
            {
                ShowErrorToUser(new string[]
                {
                    $"More than one file in directory {sourcePath} is related to DrawingLink.",
                    "Only one version of drawing link can be loaded in Rhino at the time."
                });
                return false;
            }


            var destinationPath = Environment.ExpandEnvironmentVariables(_settings.DestinationGhaPath);
            if (!Directory.Exists(destinationPath))
            {
                ShowErrorToUser(new string[]
                {
                    $"Destination directory does not exist {destinationPath}",
                });
                return false;
            }

            return true;
        }
    }
}
