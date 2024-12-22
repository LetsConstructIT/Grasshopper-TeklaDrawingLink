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
    public class ExportIfc4Component : TeklaExportComponentBase<ExportIfc4Command>
    {
        private const string _settingsExtension = "ifc4export.json";
        private ExportMode _mode = ExportMode.Selection;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportIfc4;

        public ExportIfc4Component() : base(ComponentInfos.ExportIfc4Component)
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

#if API2022 || API2023 || API2024
            if (_mode == ExportMode.Selection)
                ModelInteractor.SelectModelObjects(modelObjects);

            var jsonSettingsPath = SearchSettings(settingsName);
            if (string.IsNullOrEmpty(jsonSettingsPath))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "IFC4 export settings file was not found.");
                return;
            }

            var exportSettings = Json.Deserialize<ExportSettings>(File.ReadAllText(jsonSettingsPath));

            var status = ExportIFC(outputPath, exportSettings);
            if (!status)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "IFC4 export failed. See session log for additional info in case failure.");
                return;
            }
#else
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IFC4 export not available in this Tekla version.");
#endif

            _command.SetOutputValues(DA, outputPath);
        }
        private string SanitizePath(string path)
        {
            var absolutePath = ReplaceRelativeModelPath(path);
            var correctPath = PlaceInTheModelPathIfPlainFile(absolutePath, directory: "IFC");
            var withExtension = AddExtensionIfMissing(correctPath, extension: ".ifc");

            CreateDirectoryIfNeeded(withExtension);

            return withExtension;
        }

#if API2022 || API2023 || API2024

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

        private bool ExportIFC(string fullPath, ExportSettings settings)
        {
            var viewType = GetExportViewType(settings.ExportTypeIndex);
            var propertySets = GetPropertySets(settings.SelectedAdditionalPropertySetName);
            var basePoint = GetExportBasePoint(settings.BasePointExportIndex);
            var exportLayersAs = GetExportLayersAs(settings.SelectedExportLayerAsItem);
            var objectColoring = GetObjectColoring(settings.SelectedObjectColoring);
            var flags = GetExportFlags(settings);
            var basePointGuid = GetBasePointGuid(basePoint, settings.BasePointExportIndex);

            if (_mode == ExportMode.Selection)
                return Operation.CreateIFC4ExportFromSelected(fullPath,
                                                              viewType,
                                                              propertySets,
                                                              basePoint,
                                                              exportLayersAs,
                                                              objectColoring,
                                                              flags,
                                                              basePointGuid);
            else
                return Operation.CreateIFC4ExportFromAll(fullPath,
                                                         viewType,
                                                         propertySets,
                                                         basePoint,
                                                         exportLayersAs,
                                                         objectColoring,
                                                         flags,
                                                         basePointGuid);
        }

        private Operation.IFCExportViewTypeEnum GetExportViewType(int exportTypeIndex)
        {
            return exportTypeIndex switch
            {
                1 => Operation.IFCExportViewTypeEnum.DESIGN_TRANSFER_VIEW,
                5 => Operation.IFCExportViewTypeEnum.BRIDGE_VIEW,
                _ => Operation.IFCExportViewTypeEnum.REFERENCE_VIEW
            };
        }

        private IEnumerable<string> GetPropertySets(string selectedAdditionalPropertySetName)
        {
            if (selectedAdditionalPropertySetName == "<new>")
                return Enumerable.Empty<string>();

            var allPSets = IfcPSetsFiles.GetAdditionalPSetsFilesFromAllPossibleFolders();
            var matchingPSet = allPSets.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p.Name) == selectedAdditionalPropertySetName);
            if (matchingPSet != null)
                return new string[] { matchingPSet.FullName };
            else
                return Enumerable.Empty<string>();
        }

        private Operation.ExportBasePoint GetExportBasePoint(int basePointExportIndex)
        {
            return basePointExportIndex switch
            {
                0 => Operation.ExportBasePoint.GLOBAL,
                1 => Operation.ExportBasePoint.WORK_PLANE,
                _ => Operation.ExportBasePoint.BASE_POINT
            };
        }

        private string GetExportLayersAs(string selectedExportLayerAsItem)
        {
            return selectedExportLayerAsItem switch
            {
                "Phase" => "__Phase__",
                _ => "__Name__"
            };
        }

        private string GetObjectColoring(string selectedObjectColoring)
        {
            if (selectedObjectColoring.Equals("By object class"))
                return "ByObjectClass";
            else
                return selectedObjectColoring;
        }

        private Operation.IFCExportFlags GetExportFlags(ExportSettings settings)
        {
            var flags = new Operation.IFCExportFlags();
            if (settings.IsLocationFromOrganizer)
                flags.IsLocationFromOrganizer = true;
#if API2023 || API2024
            if (settings.BasePointExportIndex == 0)
                flags.UseIfcMapConversion = true;
#endif
            if (settings.IsFlatBeamsAsPlates)
                flags.IsFlatBeamsAsPlates = true;
            if (settings.IsPoursEnabled)
                flags.IsPoursEnabled = true;
            if (GetAreRebarSetGroupsExportedAsIndividualBars())
                flags.ExportRebarSetGroupAsIndividualBars = true;
            if (settings.IsAssembliesEnabled)
                flags.IsAssembliesEnabled = true;
            if (settings.IsBoltsEnabled)
                flags.IsBoltsEnabled = true;
            if (settings.IsWeldsEnabled)
                flags.IsWeldsEnabled = true;
            if (settings.IsGridsEnabled)
                flags.IsGridsEnabled = true;
            if (settings.IsRebarsEnabled)
                flags.IsRebarsEnabled = true;
            if (settings.IsSurfaceTreatmentsAndSurfacesEnabled)
                flags.IsSurfaceTreatmentsAndSurfacesEnabled = true;

            return flags;
        }

        private bool GetAreRebarSetGroupsExportedAsIndividualBars()
        {
            var variable = string.Empty;
            TeklaStructuresSettings.GetAdvancedOption("XS_EXPORT_IFC_REBARSET_INDIVIDUAL_BARS", ref variable);
            return variable.ToLower() == "true";
        }

        private string GetBasePointGuid(Operation.ExportBasePoint basePoint, int basePointExportIndex)
        {
            var reducedIndex = basePointExportIndex - 2;
            if (basePoint != Operation.ExportBasePoint.BASE_POINT)
                return string.Empty;

            var basePoints = ProjectInfo.GetBasePoints().OrderBy(b => b.Name).Select(b => b.Name).ToList();
            if (reducedIndex < 0 || reducedIndex >= basePoints.Count())
                return string.Empty;

            return basePoints[reducedIndex];
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
            public int LocationByIndex { get; set; }
            public int SelectionTypeIndex { get; set; }
            public string SelectedObjectColoring { get; set; }
            public string SelectedExportLayerAsItem { get; set; }
            public int ExportTypeIndex { get; set; }
            public int BasePointExportIndex { get; set; }
            public int CIPExportListIndex { get; set; }
            public string SelectedAdditionalPropertySetName { get; set; }
            public string SelectedExportFormat { get; set; }
            public int PropertySetsIndex { get; set; }
            public bool IsFlatBeamsAsPlates { get; set; }
            public bool IsLocationFromOrganizer { get; set; }
            public bool IsPoursEnabled { get; set; }
            public bool IsAssembliesEnabled { get; set; }
            public bool IsBoltsEnabled { get; set; }
            public bool IsWeldsEnabled { get; set; }
            public bool IsGridsEnabled { get; set; }
            public bool IsRebarsEnabled { get; set; }
            public bool IsSurfaceTreatmentsAndSurfacesEnabled { get; set; }
        }
    }

    public class ExportIfc4Command : CommandBase
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
