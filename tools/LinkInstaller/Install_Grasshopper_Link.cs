using LinkInstaller.Core;
using LinkInstaller.Validators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        // Directory with GrasshopperTekla*.gha files separated per different Tekla version directories
        static string _pathToSources = @"C:\GHA\SourceGHAs";

        // Path to destination directory, from which Grasshopper is reading plugins
        static string _pathToGhLibraries = @"%AppData%\Grasshopper\Libraries";

        // Location of Rhino.exe, which should be opened after installation process.
        // (Leave blank if you don't want to automatically open Rhino)
        static string _pathToRhino = @"C:\Program Files\Rhino 7\System";






        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            var arguments = BuildArguments();

            LinkInstaller.Program.Main(arguments);

            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("Grasshopper link installation completed");
        }

        private static string[] BuildArguments()
        {
            var version = GetTeklaVersion();
            var arguments = new string[]
            {
                version,
                _pathToSources,
                _pathToGhLibraries,
                _pathToRhino
            };

            return arguments;
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
    }
}




// https://github.com/eduherminio/dotnet-combine





// Constants.cs
namespace LinkInstaller
{
    public class Constants
    {
        public static readonly int MinTeklaVersion = 2021;
        public static readonly int MaxTeklaVersion = 2023;
    }
}


// GhaFileMover.cs
namespace LinkInstaller
{
    public class GhaFileMover
    {
        private string _sourceDir;
        private string _destDir;

        public GhaFileMover(string sourceDir, string destDir)
        {
            if (sourceDir == null) throw new ArgumentNullException("sourceDir");
            if (destDir == null) throw new ArgumentNullException("destDir");

            _sourceDir = sourceDir;
            _destDir = destDir;
        }

        public void MoveFiles()
        {
            var sourcePaths = Directory.EnumerateFiles(_sourceDir, "GrasshopperTekla*.gha");

            foreach (var sourcePath in sourcePaths)
            {
                var sourceFileName = Path.GetFileName(sourcePath);
                foreach (var destFileNameToRemove in GetCounterpartsFileNames(sourceFileName))
                {
                    var pathToRemove = Path.Combine(_destDir, destFileNameToRemove);
                    if (File.Exists(pathToRemove))
                        File.Delete(pathToRemove);
                }

                File.Copy(
                    sourcePath,
                    Path.Combine(_destDir, sourceFileName));
            }
        }

        private IEnumerable<string> GetCounterpartsFileNames(string fileName)
        {
            if (fileName.StartsWith("GrasshopperTeklaDrawingLink"))
            {
                return Enumerable
                    .Range(Constants.MinTeklaVersion, Constants.MaxTeklaVersion - Constants.MinTeklaVersion + 1)
                    .Select(ver => string.Format("GrasshopperTeklaDrawingLink_{0}.gha", ver));
            }
            else
                return new string[] { fileName };
        }
    }
}


// Program.cs
namespace LinkInstaller
{
    public class Program
    {
        public static void Main(string[] args)
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


// Core/FileUnblocker.cs
namespace LinkInstaller.Core
{
    public class FileUnblocker
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        public bool Unblock(string fileName)
        {
            return DeleteFile(fileName + ":Zone.Identifier");
        }
    }
}


// Core/RhinoRunner.cs
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


// Core/Settings.cs
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
                var basePath = SourceGhaPath;
                var pathMatchingVersion = Path.Combine(basePath, TeklaVersion);
                if (Directory.Exists(pathMatchingVersion))
                    return pathMatchingVersion;

                if (pathMatchingVersion.Contains(".0"))
                {
                    var pathWithoutMinor = pathMatchingVersion.Substring(0, pathMatchingVersion.LastIndexOf(".0"));
                    return pathWithoutMinor;
                }
                else if (pathMatchingVersion.Contains(".1"))
                {
                    var olderTeklasPath = pathMatchingVersion.Replace(".1", "i");
                    return olderTeklasPath;
                }
				
				return "";
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


// Validators/ArgumentsValidator.cs
namespace LinkInstaller.Validators
{
    public class ArgumentsValidator : MessageBoxValidator
    {
        private string[] _arguments;

        public ArgumentsValidator(string[] arguments)
        {
            if (arguments == null) throw new ArgumentNullException("arguments");

            _arguments = arguments;
        }

        public override bool IsValid()
        {
            if (_arguments.Length < 3)
            {
                ShowErrorToUser(new string[]
                {
                    "Too few arguments. I need:",
                    "  Tekla version",
                    "  Path to source .gha",
                    "  Path to GH 'Libraries' directory"
                });
                return false;
            }

            return true;
        }
    }
}


// Validators/DirectoriesValidator.cs
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


// Validators/MessageBoxValidator.cs
namespace LinkInstaller.Validators
{
    public abstract class MessageBoxValidator
    {
        public void ShowErrorToUser(params string[] messages)
        {
            var mergedMessage = string.Join(Environment.NewLine, messages);
            System.Windows.Forms.MessageBox.Show(mergedMessage, "Error");
        }

        public abstract bool IsValid();
    }
}


// Validators/RhinoProcessValidator.cs
namespace LinkInstaller.Validators
{
    public class RhinoProcessValidator : MessageBoxValidator
    {
        public override bool IsValid()
        {
            var isAnyRhinoRunning = Process
                .GetProcesses()
                .Any(p => p.ProcessName.StartsWith("Rhino"));

            if (isAnyRhinoRunning)
            {
                ShowErrorToUser(new string[]
                {
                    "Rhino is already running.",
                    "In order to updated .gha files you have to close it."
                });

                return false;
            }

            return true;
        }
    }
}

