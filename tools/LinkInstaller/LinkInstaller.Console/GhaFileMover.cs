using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
