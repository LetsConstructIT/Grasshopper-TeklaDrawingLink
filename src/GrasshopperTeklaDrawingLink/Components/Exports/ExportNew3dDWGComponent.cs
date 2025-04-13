using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Tekla.Structures;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using ZeroDep;

namespace GTDrawingLink.Components.Exports
{
    public class ExportNew3dDWGComponent : TeklaExportComponentBase<ExportNew3dDWGCommand>
    {
        private const string _settingsExtension = "dwgExport.json";
        private ExportMode _mode = ExportMode.Selection;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportNew3dDWG;

        public ExportNew3dDWGComponent() : base(ComponentInfos.ExportNew3dDWGComponent)
        {
            SetCustomMessage();
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (modelObjects, path, settingsName) = _command.GetInputValues();
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

#if API2020 || API2021

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "New 3D DWG export not available in this Tekla version.");
#else
            if (_mode == ExportMode.Selection)
                ModelInteractor.SelectModelObjects(modelObjects);

            var jsonSettingsPath = SearchSettings(settingsName);
            if (string.IsNullOrEmpty(jsonSettingsPath))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "3D DWG export settings file was not found.");
                return;
            }

            var exportSettings = Json.Deserialize<ExportSettings>(File.ReadAllText(jsonSettingsPath));

            var status = Export3dDwg(outputPath, exportSettings);
            if (status != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "3D DWG export failed. See session log for additional info in case failure.");
                return;
            }
#endif

            _command.SetOutputValues(DA, outputPath);
        }

        private string SanitizePath(string path)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath, directory: "3Ddwg");
            var withExtension = AddExtensionIfMissing(correctPath, extension: ".dwg");

            CreateDirectoryIfNeeded(withExtension);

            return withExtension;
        }

#if API2022 || API2023 || API2024 || API2025

        private string? SearchSettings(string settings)
        {
            if (!settings.EndsWith(_settingsExtension))
                settings += $".{_settingsExtension}";

            if (File.Exists(settings))
                return settings;

            var fileInfo = new TeklaStructuresFiles(ModelInteractor.ModelPath())
                .GetAttributeFile(settings);

            return (fileInfo != null && fileInfo.Exists) ? fileInfo.FullName : null;
        }

        private int Export3dDwg(string fullPath, ExportSettings settings)
        {
            var modelName = Path.GetFileNameWithoutExtension(fullPath);
            var directory = Path.GetDirectoryName(fullPath);
            var basePoint = GetExportBasePoint(settings.LocationByGuid);
            var objectColoring = GetObjectColoring(settings.SelectedObjectColoring);
            var exportLayersAs = GetExportLayersAs(settings.SelectedExportLayerAsItem);
            var basePointGuid = settings.LocationByGuid;

            if (_mode == ExportMode.Selection)
                return Tekla.Structures.ModelInternal.Operation.ExportDwgFromSelected(modelName, directory, basePoint, basePointGuid,
                                                              objectColoring,
                                                              exportLayersAs,
                                                              out int exportedObjects, out int exportFailedObjects, out string userName);
            else
                return Tekla.Structures.ModelInternal.Operation.ExportDwgFromAll(modelName, directory, basePoint, basePointGuid,
                                                              objectColoring,
                                                              exportLayersAs,
                                                              out int exportedObjects, out int exportFailedObjects, out string userName);
        }

        private Operation.ExportBasePoint GetExportBasePoint(string locationByGuid)
        {
            return locationByGuid switch
            {
                "0ff713ca-2fe9-497a-9534-92e0f76108a2" => Operation.ExportBasePoint.GLOBAL,
                "891b1397-def8-4c01-8a5d-0a1b27594124" => Operation.ExportBasePoint.WORK_PLANE,
                _ => Operation.ExportBasePoint.BASE_POINT
            };
        }

        private string GetExportLayersAs(string selectedExportLayerAsItem)
        {
            return selectedExportLayerAsItem switch
            {
                "Phase" => "Phase",
                _ => "Name"
            };
        }

        private string GetObjectColoring(string selectedObjectColoring)
        {
            if (selectedObjectColoring.Equals("By object class"))
                return "ByObjectClass";
            else
                return selectedObjectColoring;
        }

#endif
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

        private class ExportSettings
        {
            public string DialogType { get; set; }
            public string TargetFileName { get; set; }
            public string Folder { get; set; }
            public string LocationByGuid { get; set; }
            public int SelectionTypeIndex { get; set; }
            public string SelectedObjectColoring { get; set; }
            public string SelectedExportLayerAsItem { get; set; }
        }
    }

    public class ExportNew3dDWGCommand : CommandBase
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
