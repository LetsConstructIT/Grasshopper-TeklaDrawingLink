using System;
using System.IO;

namespace LinkInstaller.Core
{
    public class Settings
    {
        private string _destinationGhaPath;
        public string TeklaVersion { get; }
        public string SourceGhaPath { get; }
        public string RhinoPath { get; }

        public Settings(string teklaVersion, string sourceGhaPath, string destinationGhaPath, string rhinoPath)
        {
            TeklaVersion = teklaVersion;
            SourceGhaPath = sourceGhaPath;
            _destinationGhaPath = destinationGhaPath;
            RhinoPath = rhinoPath;
        }

        public string PathToTeklaVerSourceDir => Path.Combine(SourceGhaPath, TeklaVersion);
        public string DestinationGhaPath => Environment.ExpandEnvironmentVariables(_destinationGhaPath);

        public static Settings FromArguments(string[] args)
        {
            return new Settings(
                teklaVersion: args[0],
                sourceGhaPath: args[1],
                destinationGhaPath: args[2],
                rhinoPath: args.Length > 3 ? args[3] : "");
        }
    }
}
