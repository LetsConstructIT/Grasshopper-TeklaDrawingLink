using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tekla.Structures.Model;
using Tekla.Structures;

namespace GTDrawingLink.Tools
{
    internal class IfcPSetsFiles
    {
        private const string _extension = "*.xml";

        public static List<FileInfo> GetAdditionalPSetsFilesFromAllPossibleFolders()
        {
            var projectDir = string.Empty;
            var firmDir = string.Empty;
            var systemDir = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_PROJECT", ref projectDir);
            TeklaStructuresSettings.GetAdvancedOption("XS_FIRM", ref firmDir);
            TeklaStructuresSettings.GetAdvancedOption("XS_SYSTEM", ref systemDir);

            var result = new List<FileInfo>();
            GetAdditionalPSetsFilesFromFolder(new Model().GetInfo().ModelPath, result, _extension);
            GetAdditionalPSetsFilesFromFolder(projectDir, result, _extension);
            GetAdditionalPSetsFilesFromFolder(firmDir, result, _extension);
            foreach (string folder in systemDir.Split(new char[] { ';' }))
                GetAdditionalPSetsFilesFromFolder(folder, result, _extension);

#if API2026
            AddDefaultPSetsFiles(projectDir, firmDir, systemDir, result);
#endif
            return result;
        }

#if API2026
        // Default property sets of the Tekla 2026 export dialog
        private static readonly string[] _defaultPSetsFileNames =
        {
            "Reference view default properties (IFC4).xml",
            "Reference view default properties (IFC4.3).xml",
            "Design transfer view default properties (IFC4).xml",
            "Design transfer view default properties (IFC4.3).xml",
            "Bridge view default properties.xml",
            "Precast view default properties.xml",
        };

        // Tekla 2026 keeps its default property sets outside the AdditionalPSets folders,
        // in common\collaboration\ifc. The export dialog looks for them as below.
        private static void AddDefaultPSetsFiles(string projectDir, string firmDir, string systemDir, List<FileInfo> files)
        {
            var inpDir = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_INP", ref inpDir);

            var folders = new List<string> { projectDir, firmDir };
            folders.AddRange(systemDir.Split(';'));
            foreach (var folder in inpDir.Split(';').Where(f => !string.IsNullOrWhiteSpace(f)))
            {
                folders.Add(folder);
                folders.Add(Path.Combine(folder, @"..\..\common\inp"));
                folders.Add(Path.Combine(folder, @"..\..\common\collaboration\ifc"));
            }

            foreach (var folder in folders.Where(f => !string.IsNullOrWhiteSpace(f)))
            {
                foreach (var fileName in _defaultPSetsFileNames)
                {
                    var file = new FileInfo(Path.Combine(folder, fileName));
                    if (file.Exists && !files.Any(f => f.Name.ToLower() == file.Name.ToLower()))
                        files.Add(file);
                }
            }
        }
#endif

        private static void GetAdditionalPSetsFilesFromFolder(string folder, List<FileInfo> files, string extension)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(Path.Combine(folder, "AdditionalPSets")))
                return;

            var newFiles = new DirectoryInfo(Path.Combine(folder, "AdditionalPSets")).GetFiles(extension);
            for (int i = 0; i < newFiles.Length; i++)
            {
                var xml = newFiles[i];
                if (!files.Any((FileInfo file) => file.Name.ToLower() == xml.Name.ToLower()))
                    files.Add(xml);
            }
        }
    }
}
