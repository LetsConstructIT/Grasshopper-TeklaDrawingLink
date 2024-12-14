using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace GTDrawingLink.Components.Exports
{
    public class ExportReportComponent : TeklaExportComponentBase<ExportReportCommand>
    {
        private ExportMode _mode = ExportMode.Selection;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportReport;

        public ExportReportComponent() : base(ComponentInfos.ExportReportComponent)
        {
            SetCustomMessage();
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (modelObjects, path, template, title1, title2, title3) = _command.GetInputValues();
            if (_mode == ExportMode.Selection && modelObjects.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No elements on input. Provide them or change mode to All.");
                return;
            }

            if (path.EndsWith(@"\"))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid path. You must provide a file name, not directory.");
                return;
            }

            var outputPath = SanitizePath(path);

            if (_mode == ExportMode.Selection)
                ModelInteractor.SelectModelObjects(modelObjects);

            ExportReport(outputPath, template, title1, title2, title3);

            _command.SetOutputValues(DA, outputPath);
        }

        private string SanitizePath(string path)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath, directory: "Reports");
            var withExtension = AddExtensionIfMissing(correctPath, extension: ".xsr");

            CreateDirectoryIfNeeded(withExtension);

            return withExtension;
        }

        private void ExportReport(string outputPath, string templateName, string title1, string title2, string title3)
        {
            if (_mode == ExportMode.Selection)
                Operation.CreateReportFromSelected(templateName, outputPath, title1, title2, title3);
            else
                Operation.CreateReportFromAll(templateName, outputPath, title1, title2, title3);
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.ExportSelection.Name, SelectionModeMenuItem_Clicked, true, _mode == ExportMode.Selection).ToolTipText = ParamInfos.ExportSelection.Description;
            Menu_AppendItem(menu, ParamInfos.ExportAll.Name, AllModeMenuItem_Clicked, true, _mode == ExportMode.All).ToolTipText = ParamInfos.ExportAll.Description;
            Menu_AppendSeparator(menu);
        }

        private void SelectionModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = ExportMode.Selection;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void AllModeMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = ExportMode.All;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.IfcExportMode.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.IfcExportMode.Name, ref serializedInt);
            _mode = (ExportMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        private void SetCustomMessage()
        {
            Message = _mode switch
            {
                ExportMode.Selection => ParamInfos.ExportSelection.Name,
                ExportMode.All => ParamInfos.ExportAll.Name,
                _ => "",
            };
        }

        enum ExportMode
        {
            Selection = 0,
            All = 1
        }
    }

    public class ExportReportCommand : CommandBase
    {
        private readonly InputOptionalListParam<ModelObject> _inModelObjects = new InputOptionalListParam<ModelObject>(ParamInfos.ModelObject);
        private readonly InputParam<string> _inPath = new InputParam<string>(ParamInfos.ExportPath);
        private readonly InputParam<string> _inTemplate = new InputParam<string>(ParamInfos.ReportTemplate);
        private readonly InputOptionalParam<string> _inTitle1 = new InputOptionalParam<string>(ParamInfos.Title1);
        private readonly InputOptionalParam<string> _inTitle2 = new InputOptionalParam<string>(ParamInfos.Title2);
        private readonly InputOptionalParam<string> _inTitle3 = new InputOptionalParam<string>(ParamInfos.Title3);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal (List<ModelObject> modelObjects, string path, string template, string? title1, string? title2, string? title3) GetInputValues()
        {
            var modelObjects = _inModelObjects.ValueProvidedByUser ? _inModelObjects.Value : new List<ModelObject>();
            return (
                modelObjects,
                _inPath.Value,
                _inTemplate.Value,
                _inTitle1.ValueProvidedByUser ? _inTitle1.Value : string.Empty,
                _inTitle2.ValueProvidedByUser ? _inTitle2.Value : string.Empty,
                _inTitle3.ValueProvidedByUser ? _inTitle3.Value : string.Empty);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string path)
        {
            _outPath.Value = path;

            return SetOutput(DA);
        }
    }
}
