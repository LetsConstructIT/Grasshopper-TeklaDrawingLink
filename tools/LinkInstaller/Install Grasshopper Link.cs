using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        // Location of LinkInstaller.exe program
        static string _pathToLinkInstaller = @"D:\Programowanie\Grasshopper\Grasshopper-TeklaDrawingLink\tools\LinkInstaller.Console\bin\Debug";

        // Directory with GrasshopperTekla*.gha files separated per different Tekla version directories
        static string _pathToSources = @"SourceGHAs";

        // Path to destination directory, from which Grasshopper is reading plugins
        static string _pathToGhLibraries = @"%AppData%\Grasshopper\Libraries";

        // Location of Rhino.exe, which should be opened after installation process.
        // (Leave blank if you don't want to automatically open Rhino)
        static string _pathToRhino = @"C:\Program Files\Rhino 7\System";









        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            var linkInstallerExePath = GetPathToLinkInstaller();
            if (string.IsNullOrEmpty(linkInstallerExePath))
            {
                MessageBox.Show(string.Format("Could not find LinkInstaller.exe within path {0}", _pathToLinkInstaller), "Error");
                return;
            }

            var arguments = BuildArguments();

            Process.Start(linkInstallerExePath, arguments);
        }

        private static string GetPathToLinkInstaller()
        {
            var exeName = "LinkInstaller.exe";

            if (File.Exists(_pathToLinkInstaller) &&
                Path.GetFileName(_pathToLinkInstaller) == exeName)
            {
                return _pathToLinkInstaller;
            }

            return Directory.EnumerateFiles(_pathToLinkInstaller, exeName).FirstOrDefault();
        }

        private static string BuildArguments()
        {
            var version = GetTeklaVersion();

            var arguments = new string[]
            {
                version,
                _pathToSources,
                _pathToGhLibraries,
                _pathToRhino
            };

            return string.Join(" ", arguments.Select(a => SurroundWithDoubleQuotes(a)).ToArray());
        }

        private static string GetTeklaVersion()
        {
            var teklaLibrariesVersion = typeof(Model)
                .Assembly
                .GetReferencedAssemblies()
                .First((AssemblyName a) => a.Name == "Tekla.Structures").Version;

            var version = string.Format("{0}.{1}", teklaLibrariesVersion.Major, teklaLibrariesVersion.Minor);
            return version;
        }

        private static string SurroundWithDoubleQuotes(string input)
        {
            return string.Format("\"{0}\"", input);
        }
    }
}