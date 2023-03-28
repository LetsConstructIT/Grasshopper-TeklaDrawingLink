using System;
using System.Linq;
using System.IO;
using Tekla.Structures;

namespace GTDrawingLink.Tools
{
    internal class LightweightMacroBuilder
    {
        private static readonly Random _random = new Random();
        private static int _tempFileIndex = -1;
        private const int _maxTempFiles = 32;
        private const string _subdirName = "GH_DrawingLink";

        internal string SaveMacroAndReturnRelativePath(string macroContent)
        {
            var destDir = GetDestinationDirectory();

            var fileName = GetMacroFileName();

            SaveMacro(macroContent, destDir, fileName);

            return $@"..\{_subdirName}\{fileName}";
        }

        private void SaveMacro(string macroContent, string destDir, string fileName)
        {
            var pathToSave = Path.Combine(destDir, fileName);
            File.WriteAllText(pathToSave, macroContent);
        }

        private string GetDestinationDirectory()
        {
            var destDir = Path.Combine(GetMacroDirectory(), _subdirName);
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            return destDir;
        }

        private string GetMacroFileName()
        {
            Random random = _random;
            string result;

            lock (random)
            {
                if (_tempFileIndex < 0)
                    _tempFileIndex = _random.Next(0, _maxTempFiles);
                else
                    _tempFileIndex = (_tempFileIndex + 1) % _maxTempFiles;

                result = string.Format("GH_macro_{0:00}.cs", _tempFileIndex);
            }

            return result;
        }

        private string GetMacroDirectory()
        {
            var macroDirectories = "";
            TeklaStructuresSettings.GetAdvancedOption("XS_MACRO_DIRECTORY", ref macroDirectories);

            return macroDirectories
                .Replace(@"\\", @"\")
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();
        }
    }
}
