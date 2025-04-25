using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Exports
{
    public class ExportDwgComponent : TeklaExportComponentBase<ExportDwgCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportDWG;

        public ExportDwgComponent() : base(ComponentInfos.ExportDwgComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (drawing, directory, settings) = _command.GetInputValues();
            if (drawing is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No drawing on input. Provide at least one drawing to print.");
                return;
            }

            var exportDirectory = SanitizePath(directory);

            var output = ExportDwg(drawing, exportDirectory, settings);

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

        private string ExportDwg(Drawing drawing, string fullName, string settings)
        {
            var isActiveDrawing = CheckIfDrawingIsActive(drawing);

            var dwgExporterPath = GetDwgExporterCommand();
            try
            {
                if (!isActiveDrawing)
                    DrawingInteractor.DrawingHandler.SetActiveDrawing(drawing, false);

                var arg = GetExportArgs(fullName, settings);

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
                return $"ERROR: {message}";
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

            return fullName;
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
        private readonly InputParam<string> _inDirectory = new InputParam<string>(ParamInfos.ExportDirectory);
        private readonly InputParam<string> _inSettings = new InputParam<string>(ParamInfos.ExportSettings);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal (Drawing drawing, string directory, string settings) GetInputValues()
        {
            return (
                _inDrawing.Value,
                _inDirectory.Value,
                _inSettings.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string path)
        {
            _outPath.Value = path;

            return SetOutput(DA);
        }
    }
}
