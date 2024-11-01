using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.IO;

namespace GTDrawingLink.Components.Exports
{
    public abstract class TeklaExportComponentBase<T> : TeklaComponentBaseNew<T> where T : CommandBase, new()
    {
        protected TeklaExportComponentBase(GH_InstanceDescription info) : base(info) { }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

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
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(Path.GetDirectoryName(path)))
                return path;
            else
                return Path.Combine(ModelInteractor.ModelPath(), directory, path);
        }

        protected string AddExtensionIfMissing(string path, string extension)
        {
            if (HasExtension(path, extension))
                return path;
            else
                return $"{path}{extension}";
        }

        protected bool HasExtension(string path, string extension)
            => path.EndsWith(extension, StringComparison.OrdinalIgnoreCase);

        protected string ReplaceInvalidChars(string filename)
            => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        protected void CreateDirectoryIfNeeded(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}
