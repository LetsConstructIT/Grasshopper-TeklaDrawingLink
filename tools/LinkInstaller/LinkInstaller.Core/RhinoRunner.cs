using System.Diagnostics;
using System.Linq;
using System.IO;

namespace LinkInstaller.Core
{
    public class RhinoRunner
    {
        private string _rhinoPath;

        public RhinoRunner(string rhinoPath)
        {
            _rhinoPath = GetPathToRhinoExe(rhinoPath);
        }

        public void RunWithGrasshopper()
        {
            if (string.IsNullOrEmpty(_rhinoPath))
                return;

            var arguments = "/nosplash /notemplate /runscript=\"-grasshopper Window Show\"";
            Process.Start(_rhinoPath, arguments);
        }

        private string GetPathToRhinoExe(string rhinoPath)
        {
            var exeName = "Rhino.exe";

            if (string.IsNullOrEmpty(rhinoPath))
                return "";

            if (File.Exists(rhinoPath) &&
                Path.GetFileName(rhinoPath) == exeName)
            {
                return rhinoPath;
            }

            return Directory.EnumerateFiles(rhinoPath, exeName, SearchOption.AllDirectories)
                .FirstOrDefault();
        }
    }
}
