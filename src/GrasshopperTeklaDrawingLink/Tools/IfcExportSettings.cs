using System;
using System.IO;
using ZeroDep;

namespace GTDrawingLink.Tools
{
    /// <summary>
    /// Settings saved by the Tekla IFC export dialog: *.ifc4export.json, and *.ifcexport.json since Tekla 2026.
    /// </summary>
    internal class IfcExportSettings
    {
        public string DialogType { get; set; }
        public string TargetFileName { get; set; }
        public string Folder { get; set; }
        public string? LocationByGuid { get; set; }
        public int LocationByIndex { get; set; }
        public int SelectionTypeIndex { get; set; }
        public string SelectedObjectColoring { get; set; }
        public string SelectedExportLayerAsItem { get; set; }
        public int ExportTypeIndex { get; set; }
        public int BasePointExportIndex { get; set; }
        public int CIPExportListIndex { get; set; }
        public string SelectedAdditionalPropertySetName { get; set; }
        public string SelectedExportFormat { get; set; }
        /// <summary>"IFC2x3", "IFC4" or "IFC4.3". Only in settings saved by Tekla 2026 and newer.</summary>
        public string? SelectedExportVersion { get; set; }
        public int PropertySetsIndex { get; set; }
        public bool IsFlatBeamsAsPlates { get; set; }
        public bool IsLocationFromOrganizer { get; set; }
        public bool IsPoursEnabled { get; set; }
        public bool IsAssembliesEnabled { get; set; }
        /// <summary>Only in settings saved by Tekla 2026 and newer.</summary>
        public bool? IncludeAssemblyHierarchy { get; set; }
        public bool IsExcludeSinglePartAssemblies { get; set; }
        public bool IsBoltsEnabled { get; set; }
        public bool IsWeldsEnabled { get; set; }
        public bool IsGridsEnabled { get; set; }
        public bool IsRebarsEnabled { get; set; }
        public bool IsSurfaceTreatmentsAndSurfacesEnabled { get; set; }
        public bool IsSpacesEnabled { get; set; }

        public bool IsExportVersion(string version)
            => string.Equals(SelectedExportVersion, version, StringComparison.OrdinalIgnoreCase);

        public static IfcExportSettings Load(string path)
            => Json.Deserialize<IfcExportSettings>(File.ReadAllText(path));
    }
}
