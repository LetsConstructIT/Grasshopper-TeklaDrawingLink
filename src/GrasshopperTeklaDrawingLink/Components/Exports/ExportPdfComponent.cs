using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using Tekla.Structures;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Exports
{
    public class ExportPdfComponent : TeklaExportComponentBase<ExportPdfCommand>
    {
        private const string _settingsExtension = "PdfPrintOptions.xml";

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportPdf;

        public ExportPdfComponent() : base(ComponentInfos.ExportPdfComponent)
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

            var settingsPath = SearchSettings(settings);
            if (settingsPath is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Setting were not found, provide correct name or full path.");
                return;
            }
            var includeRevision = false;
            var exportPath = SanitizePath(drawing, path, includeRevision);

            ExportPdf(drawing, exportPath, settingsPath);

            _command.SetOutputValues(DA, exportPath);
        }

        private string? SearchSettings(string settings)
        {
            if (!settings.EndsWith(_settingsExtension))
                settings += $".{_settingsExtension}";

            if (File.Exists(settings))
                return settings;

            var fileInfo = new TeklaStructuresFiles(ModelInteractor.ModelPath())
                .GetAttributeFile(settings);

            return fileInfo.Exists ? fileInfo.FullName : null;
        }

        private string SanitizePath(Drawing drawing, string path, bool includeRevision)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath == "Plotfiles" ? "" : absolutePath, directory: "Plotfiles");

            if (!HasExtension(correctPath, ".pdf"))
            {
                var fileName = $"{drawing.GetPlotFileName(includeRevision)}.pdf";
                correctPath = Path.Combine(correctPath, fileName);
            }

            CreateDirectoryIfNeeded(correctPath);

            return correctPath;
        }

        private void ExportPdf(Drawing drawing, string fullName, string settings)
        {
            var isActiveDrawing = CheckIfDrawingIsActive(drawing);

            var dpmPath = GetDPMPrinterCommand();
            try
            {
                if (!isActiveDrawing)
                    DrawingInteractor.DrawingHandler.SetActiveDrawing(drawing, false);

                var arg = GetPrinterArgs(settings, fullName);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = dpmPath;
                startInfo.Arguments = arg;

                Process proc = new Process() { StartInfo = startInfo };
                proc.Start();
                proc.WaitForExit();
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
        }

        private bool CheckIfDrawingIsActive(Drawing drawing)
        {
            var activeDrawing = DrawingInteractor.DrawingHandler.GetActiveDrawing();
            if (activeDrawing == null)
                return false;

            return activeDrawing.GetId() == drawing.GetId();
        }

        private string GetDPMPrinterCommand()
        {
            var dpmPath = @"applications\Tekla\Model\DPMPrinter\DPMPrinterCommand.exe";
            var binPath = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSBIN", ref binPath);

            return Path.Combine(binPath.Replace(@"\\", @"\"), dpmPath);
        }
        private string GetPrinterArgs(string printerTemplate, string outputPath)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append("printActive:true ");
            arg.Append("printer:pdf ");
            arg.AppendFormat(@"settingsFile:""{0}"" ", printerTemplate);
            arg.AppendFormat(@"out:""{0}"" ", outputPath);

            return arg.ToString();
        }
    }

    public class ExportPdfCommand : CommandBase
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
