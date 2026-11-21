using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tekla.Structures;

namespace GTDrawingLink.Components.Exports
{
    public abstract class TeklaExportComponentBase<T> : TeklaComponentBaseNew<T> where T : CommandBase, new()
    {
        protected TeklaExportComponentBase(GH_InstanceDescription info) : base(info) { }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
            Menu_AppendSeparator(menu);
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
            if (HasAnyExtension(path))
                return path;
            else
                return $"{path}{extension}";
        }

        protected bool HasExtension(string path, string extension)
            => path.EndsWith(extension, StringComparison.OrdinalIgnoreCase);

        protected bool HasAnyExtension(string path)
            => Path.HasExtension(path);

        protected string ReplaceInvalidChars(string filename)
            => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        protected void CreateDirectoryIfNeeded(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        /// <summary>
        /// File names to look for: the settings as given when it already ends with one of the extensions, otherwise settings + each extension.
        /// </summary>
        protected IEnumerable<string> GetSettingsFileNames(string settings, IEnumerable<string> extensions)
        {
            if (extensions.Any(e => settings.EndsWith(e, StringComparison.OrdinalIgnoreCase)))
                return new[] { settings };

            return extensions.Select(e => $"{settings}.{e}");
        }

        /// <summary>
        /// First of the files that exists as a path, or in the model, project, firm or system folders.
        /// </summary>
        protected string? SearchSettingsFile(IEnumerable<string> fileNames)
        {
            var modelFiles = new TeklaStructuresFiles(ModelInteractor.ModelPath());
            foreach (var fileName in fileNames)
            {
                if (File.Exists(fileName))
                    return fileName;

                var fileInfo = modelFiles.GetAttributeFile(fileName);
                if (fileInfo != null && fileInfo.Exists)
                    return fileInfo.FullName;
            }

            return null;
        }
    }
}
