using LinkInstaller.Core;
using System;
using System.IO;
using System.Linq;

namespace LinkInstaller.Validators
{
    public class DirectoriesValidator : MessageBoxValidator
    {
        private Settings _settings;

        public DirectoriesValidator(Settings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            _settings = settings;
        }

        public override bool IsValid()
        {
            var sourcePath = _settings.PathToTeklaVerSourceDir;
            if (!Directory.Exists(sourcePath))
            {
                ShowErrorToUser(new string[]
                {
                    string.Format("Subdirectory for Tekla version {0} was not found",_settings.TeklaVersion),
                    string.Format("in directory {0}.",_settings.SourceGhaPath),
                    "",
                    "Check its existence and modify startup Tekla macro."
                });
                return false;
            }

            if (!Directory.EnumerateFiles(sourcePath, "*.gha").Any())
            {
                ShowErrorToUser(new string[]
                {
                    string.Format("Source directory {0} does not have .gha files.",sourcePath),
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
                    string.Format("More than one file in directory {0} is related to DrawingLink.",sourcePath),
                    "Only one version of drawing link can be loaded in Rhino at the time."
                });
                return false;
            }


            var destinationPath = Environment.ExpandEnvironmentVariables(_settings.DestinationGhaPath);
            if (!Directory.Exists(destinationPath))
            {
                ShowErrorToUser(new string[]
                {
                    string.Format("Destination directory does not exist {0}",destinationPath),
                });
                return false;
            }

            return true;
        }
    }
}
