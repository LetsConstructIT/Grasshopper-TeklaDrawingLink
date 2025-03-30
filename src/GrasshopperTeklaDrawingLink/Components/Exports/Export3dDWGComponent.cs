using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Exports
{
    public class Export3dDwgComponent : TeklaExportComponentBase<Export3dDwgCommand>
    {
        private ExportMode _mode = ExportMode.Selection;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.Export3dDWG;

        public Export3dDwgComponent() : base(ComponentInfos.Export3dDwgComponent)
        {
            SetCustomMessage();
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (modelObjects, path, settings) = _command.GetInputValues();
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

            Export3dDwg(outputPath, settings);

            _command.SetOutputValues(DA, outputPath);
        }

        private string SanitizePath(string path)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath, directory: "3Ddwg");

            CreateDirectoryIfNeeded(correctPath);

            return correctPath;
        }

        private void Export3dDwg(string outputFileName, string settings)
        {
            var componentInput = new ComponentInput();
            componentInput.AddOneInputPosition(new Tekla.Structures.Geometry3d.Point(0, 0, 0));

            var comp = new Component(componentInput)
            {
                Name = "Tekla Structures DWG-DXF Export",
                Number = 440000004
            };

            comp.LoadAttributesFromFile(settings);
            comp.SetAttribute("outfile", outputFileName);
            comp.SetAttribute("includedobjects", (int)_mode);

            comp.Insert();
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
            Selection = 1,
            All = 0
        }
    }

    public class Export3dDwgCommand : CommandBase
    {
        private readonly InputOptionalListParam<ModelObject> _inModelObjects = new InputOptionalListParam<ModelObject>(ParamInfos.ModelObject);
        private readonly InputParam<string> _inPath = new InputParam<string>(ParamInfos.ExportPath);
        private readonly InputParam<string> _inSettings = new InputParam<string>(ParamInfos.ExportSettings);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal (List<ModelObject> modelObjects, string path, string settings) GetInputValues()
        {
            var modelObjects = _inModelObjects.ValueProvidedByUser ? _inModelObjects.Value : new List<ModelObject>();
            return (
                modelObjects,
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
