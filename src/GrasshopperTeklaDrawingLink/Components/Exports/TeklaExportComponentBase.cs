using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.IO;

namespace GTDrawingLink.Components.Exports
{
    public abstract class TeklaExportComponentBase<T> : TeklaComponentBaseNew<T> where T : CommandBase, new()
    {
        protected TeklaExportComponentBase(GH_InstanceDescription info) : base(info) { }

        protected string CreateDirectoryIfNeeded(string path)
        {
            path = path.Replace("/","_");

            var modelPath = $"{ModelInteractor.ModelPath()}\\";
            if (path.StartsWith(".\\"))
                path = path.Replace(".\\", modelPath);

            if (Directory.Exists(path))
                return path;

            if (File.Exists(path))
                return path;

            var directoryPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            return path;
        }
    }
}
