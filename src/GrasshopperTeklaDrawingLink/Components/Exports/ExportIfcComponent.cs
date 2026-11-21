using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Exports
{
    public class ExportIfcComponent : TeklaExportComponentBase<ExportIfcCommand>
    {
        private ExportMode _mode = ExportMode.Selection;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportIfc;

        public ExportIfcComponent() : base(ComponentInfos.ExportIfcComponent)
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

            ExportIFC(outputPath, settings);

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

        private void ExportIFC(string outputFileName, string settings)
        {
            var componentInput = new ComponentInput();
            componentInput.AddOneInputPosition(new Tekla.Structures.Geometry3d.Point(0, 0, 0));

            var comp = new Component(componentInput)
            {
                Name = "ExportIFC",
                Number = BaseComponent.PLUGIN_OBJECT_NUMBER
            };

#if API2026
            LoadSettings(comp, settings, outputFileName);
#else
            comp.LoadAttributesFromFile(settings);
#endif
            comp.SetAttribute("OutputFile", outputFileName);
            comp.SetAttribute("CreateAll", (int)_mode);

            comp.Insert();
        }

#if API2026
        // Tekla 2026 merged the IFC2x3 and IFC4 export dialogs and saves their settings as *.ifcexport.json.
        // Earlier versions saved IFC2x3 settings for this plugin as *.IFCExportPlugin.MainDialog.xml.
        private const string _settingsExtension = "ifcexport.json";
        private const string _pluginSettingsExtension = "IFCExportPlugin.MainDialog.xml";
        private const string _modelOriginGuid = "0ff713ca-2fe9-497a-9534-92e0f76108a2";
        private const string _workPlaneGuid = "891b1397-def8-4c01-8a5d-0a1b27594124";

        private void LoadSettings(Component comp, string settings, string outputFileName)
        {
            var jsonSettingsPath = SearchSettingsFile(GetSettingsFileNames(settings, new[] { _settingsExtension }));
            var exportSettings = jsonSettingsPath != null ? IfcExportSettings.Load(jsonSettingsPath) : null;
            if (exportSettings != null && exportSettings.IsExportVersion("IFC2x3"))
            {
                SetAttributes(comp, exportSettings, outputFileName);
                return;
            }

            var pluginSettingsPath = SearchSettingsFile(GetSettingsFileNames(settings, new[] { _pluginSettingsExtension }));
            if (pluginSettingsPath == null)
            {
                var message = exportSettings != null
                    ? $"Export settings '{settings}' are for {exportSettings.SelectedExportVersion ?? "IFC4"}, not IFC2x3. Exported with default settings. Use Export IFC4, or save IFC2x3 settings in the Tekla export dialog."
                    : $"Export settings '{settings}' were not found. Exported with default settings.";
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
            }

            comp.LoadAttributesFromFile(settings);

            // Tekla 2026 reads "Assemblies" as "selected objects and their assemblies", so the selection export
            // would contain the whole assemblies. Earlier versions only added the assembly hierarchy.
            if (pluginSettingsPath != null && AreAssembliesEnabled(pluginSettingsPath))
            {
                comp.SetAttribute("Assemblies", 0);
                comp.SetAttribute("InclAssemblyHierarc", 1);
            }
        }

        private bool AreAssembliesEnabled(string pluginSettingsPath)
        {
            var value = XDocument.Load(pluginSettingsPath).Root?.Element("Assemblies")?.Value;
            return int.TryParse(value, out var assemblies) && assemblies > 0;
        }

        private void SetAttributes(Component comp, IfcExportSettings settings, string outputFileName)
        {
            // The same values the Tekla 2026 export dialog passes to this plugin
            comp.SetAttribute("Assemblies", 0);
            comp.SetAttribute("InclAssemblyHierarc", ToInt(settings.IncludeAssemblyHierarchy == true));
            comp.SetAttribute("CreateSelectedAndAs", 0);
            comp.SetAttribute("FilterName", string.Empty);
            comp.SetAttribute("ExcludeSnglPrtAsmb", ToInt(settings.IsExcludeSinglePartAssemblies));
            comp.SetAttribute("ReinforcingBars", ToInt(settings.IsRebarsEnabled));
            comp.SetAttribute("Bolts", ToInt(settings.IsBoltsEnabled));
            comp.SetAttribute("Welds", ToInt(settings.IsWeldsEnabled));
            comp.SetAttribute("GridExport", ToInt(settings.IsGridsEnabled));
            comp.SetAttribute("PourObjects", ToInt(settings.IsPoursEnabled));
            comp.SetAttribute("SurfaceTreatments", ToInt(settings.IsSurfaceTreatmentsAndSurfacesEnabled));
            comp.SetAttribute("Spaces", ToInt(settings.IsSpacesEnabled));
            comp.SetAttribute("LocsFromOrganizer", ToInt(settings.IsLocationFromOrganizer));
            comp.SetAttribute("PLprofileToPlate", ToInt(settings.IsFlatBeamsAsPlates));
            comp.SetAttribute("LayersNameAsPart", ToInt(settings.SelectedExportLayerAsItem == "Layers names as part names"));
            comp.SetAttribute("ViewColors", ToInt(settings.SelectedObjectColoring == "Use current view colors"));
            comp.SetAttribute("ExportType", GetExportType(settings.ExportTypeIndex));
            comp.SetAttribute("Format", GetFormat(outputFileName));
            comp.SetAttribute("AdditionalPSets", settings.SelectedAdditionalPropertySetName ?? string.Empty);

            var (locationBy, basePointName) = GetLocation(settings.LocationByGuid);
            comp.SetAttribute("LocationBy", locationBy);
            comp.SetAttribute("BasePointName", basePointName);

            comp.SetAttribute("SectionedSpine", 0);
            comp.SetAttribute("PSetContent", 0);
            comp.SetAttribute("BuildingHierarchy", 0);
            comp.SetAttribute("BrepAsExactSolid", 0);
            comp.SetAttribute("AttrOvrdSetDataFile", string.Empty);
            comp.SetAttribute("AttrOvrdDataFile", string.Empty);
        }

        private static int ToInt(bool value) => value ? 1 : 0;

        private int GetExportType(int exportTypeIndex)
        {
            // Dialog order: Coordination view 1.0, Coordination view 2.0, Surface geometry, Steel fabrication view.
            // Plugin values: 0 = Coordination view 2.0, 1 = Surface geometry, 2 = Steel fabrication view, 3 = Coordination view 1.0
            return exportTypeIndex switch
            {
                0 => 3,
                2 => 1,
                3 => 2,
                _ => 0
            };
        }

        private int GetFormat(string outputFileName)
        {
            // The output path decides the format: 0 = ifc, 1 = ifcXML, 2 = ifcZIP
            if (HasExtension(outputFileName, ".ifcxml"))
                return 1;
            if (HasExtension(outputFileName, ".ifczip"))
                return 2;
            return 0;
        }

        private (int locationBy, string basePointName) GetLocation(string? locationByGuid)
        {
            // 0 = model origin, 1 = work plane, -1 = named base point
            if (string.IsNullOrEmpty(locationByGuid) || string.Equals(locationByGuid, _modelOriginGuid, StringComparison.OrdinalIgnoreCase))
                return (0, string.Empty);

            if (string.Equals(locationByGuid, _workPlaneGuid, StringComparison.OrdinalIgnoreCase))
                return (1, string.Empty);

            var basePoint = ProjectInfo.GetBasePoints()
                .FirstOrDefault(b => string.Equals(b.Guid.ToString(), locationByGuid, StringComparison.OrdinalIgnoreCase));
            if (basePoint != null)
                return (-1, basePoint.Name);

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Base point from export settings was not found in the model. Model origin used instead.");
            return (0, string.Empty);
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

    public class ExportIfcCommand : CommandBase
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
