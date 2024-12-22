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

            return result;
        }

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
