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

namespace GTDrawingLink.Components.Exports
{
    public class ExportIfc4Component : TeklaExportComponentBase<ExportIfc4Command>
    {
#if API2026
        // Tekla 2026 merged the IFC2x3 and IFC4 export dialogs and saves their settings as *.ifcexport.json.
        // Settings saved by earlier versions (*.ifc4export.json) are still accepted.
        private static readonly string[] _settingsExtensions = { "ifcexport.json", "ifc4export.json" };
#else
        private static readonly string[] _settingsExtensions = { "ifc4export.json" };
#endif
        private const string _modelOriginGuid = "0ff713ca-2fe9-497a-9534-92e0f76108a2";
        private const string _workPlaneGuid = "891b1397-def8-4c01-8a5d-0a1b27594124";
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

#if API2020 || API2021

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "IFC4 export not available in this Tekla version.");
#else
            if (_mode == ExportMode.Selection)
                ModelInteractor.SelectModelObjects(modelObjects);

            var settingsFileNames = GetSettingsFileNames(settingsName, _settingsExtensions);
            var jsonSettingsPath = SearchSettingsFile(settingsFileNames);
            if (string.IsNullOrEmpty(jsonSettingsPath))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"IFC4 export settings file was not found. Searched for: {string.Join(", ", settingsFileNames)}");
                return;
            }

            var exportSettings = IfcExportSettings.Load(jsonSettingsPath);
#if API2026
            if (GetExportVersion(exportSettings) == Operation.IFCExportVersionEnum.IFC2x3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "These export settings are for IFC2x3. This component exports IFC4 and IFC4.3 only.");
                return;
            }
#endif

            var status = ExportIFC(outputPath, exportSettings);
            if (!status)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "IFC4 export failed. See session log for additional info in case failure.");
                return;
            }
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

#if API2022 || API2023 || API2024 || API2025 || API2026

        private bool ExportIFC(string fullPath, IfcExportSettings settings)
        {
            var viewType = GetExportViewType(settings);
            var propertySets = GetPropertySets(settings.SelectedAdditionalPropertySetName);
            var (basePoint, basePointGuid) = GetExportLocation(settings);
            var exportLayersAs = GetExportLayersAs(settings.SelectedExportLayerAsItem);
            var objectColoring = GetObjectColoring(settings.SelectedObjectColoring);
            var flags = GetExportFlags(settings);

#if API2026
            var exportVersion = GetExportVersion(settings);
            var attributeOverrides = new List<string>();

            if (_mode == ExportMode.Selection)
                return Operation.CreateIFC4ExportFromSelected(fullPath,
                                                              viewType,
                                                              exportVersion,
                                                              attributeOverrides,
                                                              propertySets,
                                                              basePoint,
                                                              exportLayersAs,
                                                              objectColoring,
                                                              flags,
                                                              basePointGuid);
            else
                return Operation.CreateIFC4ExportFromAll(fullPath,
                                                         viewType,
                                                         exportVersion,
                                                         attributeOverrides,
                                                         propertySets,
                                                         basePoint,
                                                         exportLayersAs,
                                                         objectColoring,
                                                         flags,
                                                         basePointGuid);
#else
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
#endif
        }

#if API2026
        private Operation.IFCExportVersionEnum GetExportVersion(IfcExportSettings settings)
        {
            // Settings from earlier versions have no version and are IFC4
            if (settings.IsExportVersion("IFC2x3"))
                return Operation.IFCExportVersionEnum.IFC2x3;

            if (settings.IsExportVersion("IFC4.3"))
                return Operation.IFCExportVersionEnum.IFC43;

            return Operation.IFCExportVersionEnum.IFC4;
        }
#endif

        private Operation.IFCExportViewTypeEnum GetExportViewType(IfcExportSettings settings)
        {
#if API2026
            // The Tekla 2026 dialog lists a different set of views for each IFC version
            if (settings.SelectedExportVersion != null)
            {
                if (GetExportVersion(settings) == Operation.IFCExportVersionEnum.IFC43)
                    return settings.ExportTypeIndex switch
                    {
                        1 => Operation.IFCExportViewTypeEnum.DESIGN_TRANSFER_VIEW,
                        2 => Operation.IFCExportViewTypeEnum.BRIDGE_VIEW,
                        _ => Operation.IFCExportViewTypeEnum.REFERENCE_VIEW
                    };

                return settings.ExportTypeIndex switch
                {
                    1 => Operation.IFCExportViewTypeEnum.DESIGN_TRANSFER_VIEW,
                    2 => Operation.IFCExportViewTypeEnum.PRECAST_VIEW,
                    3 => Operation.IFCExportViewTypeEnum.MEP_REFERENCE_VIEW,
                    4 => Operation.IFCExportViewTypeEnum.MEP_DESIGN_TRANSFER_VIEW,
                    _ => Operation.IFCExportViewTypeEnum.REFERENCE_VIEW
                };
            }
#endif
            return settings.ExportTypeIndex switch
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

#if API2026
            // Names in angle brackets are the dialog's "<none>" and "<new>" entries
            if (!string.IsNullOrEmpty(selectedAdditionalPropertySetName) && !selectedAdditionalPropertySetName.StartsWith("<"))
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Property set '{selectedAdditionalPropertySetName}' from export settings was not found. Exported without it.");
#endif
            return Enumerable.Empty<string>();
        }

        private (Operation.ExportBasePoint basePoint, string basePointGuid) GetExportLocation(IfcExportSettings settings)
        {
            var locationGuid = settings.LocationByGuid ?? GetLocationGuidFromIndex(settings.LocationByIndex);

            if (string.Equals(locationGuid, _modelOriginGuid, StringComparison.OrdinalIgnoreCase))
                return (Operation.ExportBasePoint.GLOBAL, string.Empty);

            if (string.Equals(locationGuid, _workPlaneGuid, StringComparison.OrdinalIgnoreCase))
                return (Operation.ExportBasePoint.WORK_PLANE, string.Empty);

            var basePoint = ProjectInfo.GetBasePoints()
                .FirstOrDefault(b => string.Equals(b.Guid.ToString(), locationGuid, StringComparison.OrdinalIgnoreCase));
            if (basePoint != null)
                return (Operation.ExportBasePoint.BASE_POINT, basePoint.Guid.ToString());

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Base point from export settings was not found in the model. Model origin used instead.");
            return (Operation.ExportBasePoint.GLOBAL, string.Empty);
        }

        private string GetLocationGuidFromIndex(int locationByIndex)
        {
            // Legacy settings: Model origin, Work plane, then base points in model order
            var locations = new List<string> { _modelOriginGuid, _workPlaneGuid };
            locations.AddRange(ProjectInfo.GetBasePoints().Select(b => b.Guid.ToString()));

            return (locationByIndex >= 0 && locationByIndex < locations.Count) ? locations[locationByIndex] : _modelOriginGuid;
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

        private Operation.IFCExportFlags GetExportFlags(IfcExportSettings settings)
        {
            var flags = new Operation.IFCExportFlags();
            if (settings.IsLocationFromOrganizer)
                flags.IsLocationFromOrganizer = true;
#if API2023 || API2024 || API2025 || API2026
            // Must be set explicitly: Tekla treats an unset (null) value as true,
            // so leaving it unset would always export IfcMapConversion.
            // 0 = IfcMapConversion, 1 = IfcSite coordinate system
            flags.UseIfcMapConversion = settings.BasePointExportIndex == 0;
#endif
            if (settings.IsFlatBeamsAsPlates)
                flags.IsFlatBeamsAsPlates = true;
            if (settings.IsPoursEnabled)
                flags.IsPoursEnabled = true;
            if (GetAreRebarSetGroupsExportedAsIndividualBars())
                flags.ExportRebarSetGroupAsIndividualBars = true;
#if API2026
            // Tekla 2026 ignores IsAssembliesEnabled; IncludeAssemblyHierarchy replaces it.
            // Settings from earlier versions only have IsAssembliesEnabled.
            if (settings.IncludeAssemblyHierarchy ?? settings.IsAssembliesEnabled)
                flags.IncludeAssemblyHierarchy = true;
#else
            if (settings.IsAssembliesEnabled)
                flags.IsAssembliesEnabled = true;
#endif
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
