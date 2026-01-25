using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Exports
{
    public class ExportDwgComponent : TeklaExportComponentBase<ExportDwgCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportDWG;

        private readonly string[] _allowedExtensions = new string[] {".dwg", ".dxf", ".dgn" };

        public ExportDwgComponent() : base(ComponentInfos.ExportDwgComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (drawing, path, settings) = _command.GetInputValues();
            if (drawing is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No drawing on input. Provide at least one drawing to print.");
                return;
            }

            var exportPath = SanitizePath(path);

            var timeBeforeExport = DateTime.UtcNow;
            var status = ExportDwg(drawing, exportPath, settings);

            var output = exportPath;
            if (status)
                output = RenameIfNeeded(exportPath, timeBeforeExport);

            _command.SetOutputValues(DA, output);
        }

        private string SanitizePath(string path)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath == "Plotfiles" ? "" : absolutePath, directory: "Plotfiles");

            CreateDirectoryIfNeeded(correctPath);

            if (correctPath.EndsWith(@"\"))
                correctPath = correctPath.Substring(0, correctPath.Length - 1);

            return correctPath;
        }

        private bool ExportDwg(Drawing drawing, string fullName, string settings)
        {
            var isActiveDrawing = CheckIfDrawingIsActive(drawing);

            var dwgExporterPath = GetDwgExporterCommand();

            var directoryName = GetDirectoryName(fullName);
            try
            {
                if (!isActiveDrawing)
                    DrawingInteractor.DrawingHandler.SetActiveDrawing(drawing, false);

                var arg = GetExportArgs(directoryName, settings);

                var startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = dwgExporterPath,
                    Arguments = arg
                };

                Process proc = new Process() { StartInfo = startInfo };
                proc.Start();
                proc.WaitForExit();
            }
            catch (Tekla.Structures.Drawing.CannotPerformOperationDrawingNotUpToDateException)
            {
                var message = $"Drawing {drawing.Mark} was not exported due to not being up to date";
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!isActiveDrawing)
                    DrawingInteractor.DrawingHandler.CloseActiveDrawing();
            }

            return true;
        }

        private string GetDirectoryName(string fullName)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(fullName)))
                return fullName;
            else
                return Path.GetDirectoryName(fullName);
        }

        private string RenameIfNeeded(string exportPath, DateTime timeBeforeExport)
        {
            var dirInfo = new DirectoryInfo(GetDirectoryName(exportPath));
            var filesCreatedAfter = dirInfo
                .GetFiles()
                .Where(f => _allowedExtensions.Contains(f.Extension) &&
                            f.LastWriteTimeUtc >= timeBeforeExport).ToList();

            if (!Path.HasExtension(exportPath))
            {
                if (filesCreatedAfter.Count == 1)
                    return filesCreatedAfter.First().FullName;
                else
                    return exportPath;
            }

            if (filesCreatedAfter.Count != 1)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Renaming of DWG file failed for {exportPath}");
                return exportPath;
            }

            if (File.Exists(exportPath))
                File.Delete(exportPath);

            File.Move(filesCreatedAfter.First().FullName, exportPath);

            return exportPath;
        }

        private bool CheckIfDrawingIsActive(Drawing drawing)
        {
            var activeDrawing = DrawingInteractor.DrawingHandler.GetActiveDrawing();
            if (activeDrawing == null)
                return false;

            return activeDrawing.GetId() == drawing.GetId();
        }

        private string GetDwgExporterCommand()
        {
            var dpmPath = @"applications\Tekla\Drawings\DwgExport\Dwg.exe";
            var binPath = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSBIN", ref binPath);

            return Path.Combine(binPath.Replace(@"\\", @"\"), dpmPath);
        }

        private string GetExportArgs(string outputDir, string settings)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append("export ");
            arg.AppendFormat(@"outputDirectory=""{0}"" ", outputDir);
            if (!string.IsNullOrWhiteSpace(settings) && settings != "standard")
                arg.AppendFormat(@"settingFile=""{0}"" ", settings);

            return arg.ToString();
        }
    }

    public class ExportDwgCommand : CommandBase
    {
        private readonly InputParam<Drawing> _inDrawing = new InputParam<Drawing>(ParamInfos.Drawing);
        private readonly InputParam<string> _inPath = new InputParam<string>(ParamInfos.ExportPath);
        private readonly InputParam<string> _inSettings = new InputParam<string>(ParamInfos.ExportSettings);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal (Drawing drawing, string path, string settings) GetInputValues()
        {
            return (
                _inDrawing.Value,
                _inPath.Value,
                _inSettings.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string path)
        {
            _outPath.Value = path;

            return SetOutput(DA);
        }
    }
}
