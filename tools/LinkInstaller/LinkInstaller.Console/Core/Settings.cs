using System;
using System.IO;

namespace LinkInstaller.Core
{
    public class Settings
    {
        private string _destinationGhaPath;
        public string TeklaVersion { get; private set; }
        public string SourceGhaPath { get; private set; }
        public string RhinoPath { get; private set; }

        public Settings(string teklaVersion, string sourceGhaPath, string destinationGhaPath, string rhinoPath)
        {
            TeklaVersion = teklaVersion;
            SourceGhaPath = sourceGhaPath;
            _destinationGhaPath = destinationGhaPath;
            RhinoPath = rhinoPath;
        }

        public string PathToTeklaVerSourceDir
        {
            get
            {
                var basePath = Path.IsPathRooted(SourceGhaPath) ?
                    SourceGhaPath :
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SourceGhaPath);

                var pathMatchingVersion = Path.Combine(basePath, TeklaVersion);
                if (Directory.Exists(pathMatchingVersion))
                    return pathMatchingVersion;

                var pathWithoutMinor = pathMatchingVersion.Substring(0, pathMatchingVersion.LastIndexOf(".0"));
                return pathWithoutMinor;
            }
        }
        public string DestinationGhaPath
        {
            get
            {
                return Environment.ExpandEnvironmentVariables(_destinationGhaPath);
            }
        }

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
