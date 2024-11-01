using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.IO;

namespace GTDrawingLink.Components.Exports
{
    public abstract class TeklaExportComponentBase<T> : TeklaComponentBaseNew<T> where T : CommandBase, new()
    {
        protected TeklaExportComponentBase(GH_InstanceDescription info) : base(info) { }

        protected string ReplaceRelativeModelPath(string path)
        {
            var pathWithCorrectSeparators = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            if (pathWithCorrectSeparators.StartsWith(@".\"))
                return pathWithCorrectSeparators.Replace(@".\", $"{ModelInteractor.ModelPath()}\\");
            else
                return pathWithCorrectSeparators;
        }

        protected string PlaceInTheModelPathIfPlainFile(string path, string directory)
        {
            if (!string.IsNullOrEmpty(Path.GetDirectoryName(path)))
                return path;
            else
                return Path.Combine(ModelInteractor.ModelPath(), directory, path);
        }

        protected string CreateDirectoryIfNeeded(string path)
        {
            path = path.Replace("/", "_");

            var modelPath = $"{ModelInteractor.ModelPath()}\\";
            if (path.StartsWith(".\\"))
                path = path.Replace(".\\", modelPath);
            else if (path.StartsWith("./"))
                path = path.Replace("./", modelPath);

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
