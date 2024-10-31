using Grasshopper.Kernel;
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
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportPdf;

        public ExportPdfComponent() : base(ComponentInfos.ExportPdfComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (drawing, path, settings) = _command.GetInputValues();

            var directory = SanitizePath(path);

            var outputPath = ExportPdf(drawing, directory, settings);

            _command.SetOutputValues(DA, outputPath);
        }

        private string SanitizePath(string path)
        {
            var initial = CreateDirectoryIfNeeded(path);

            if (Directory.Exists(Path.GetDirectoryName(initial)))
                return initial;
            else
                return $"{ModelInteractor.ModelPath()}\\{initial}";
        }

        private string ExportPdf(Drawing drawing, string directoryPath, string settings)
        {
            var dpmPath = GetDPMPrinterCommand();
            var fullName = directoryPath;
            try
            {
                DrawingInteractor.DrawingHandler.SetActiveDrawing(drawing, false);

                var fileName = string.Format("{0}.pdf", drawing.GetPlotFileNameExt(IncludeRevisionMarkEnum.Never));
                fullName = Path.Combine(directoryPath, fileName);
                var arg = GetPrinterArgs(settings) + string.Format(@"out:""{0}""", fullName);

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
                DrawingInteractor.DrawingHandler.CloseActiveDrawing();
                throw;
            }

            return fullName;
        }

        private string GetDPMPrinterCommand()
        {
            var dpmPath = @"applications\Tekla\Model\DPMPrinter\DPMPrinterCommand.exe";
            var binPath = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XSBIN", ref binPath);

            return Path.Combine(binPath.Replace(@"\\", @"\"), dpmPath);
        }
        private string GetPrinterArgs(string printerTemplate)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append("printActive:true ");
            arg.Append("printer:pdf ");
            arg.AppendFormat(@"settingsFile:""{0}"" ", printerTemplate);

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
