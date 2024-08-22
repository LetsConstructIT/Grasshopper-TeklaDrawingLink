using System;
using System.ComponentModel;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

namespace GTDrawingLink.Components.Params.ValueLists
{
    public class AutoSettingsValueList : GH_ValueList
    {
        private AutoSettingsMode _mode;
        public override Guid ComponentGuid => new Guid("97DB9528-FA97-454F-A902-0AB465C572A4");
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AutoSettings;

        public AutoSettingsValueList() : base()
        {
            Name = "Auto Settings List";
            NickName = "AL";
            Description = "Pick object type and automatically get available Tekla settings";

            Category = VersionSpecificConstants.TabHeading;
            SubCategory = ComponentInfos.PanelHeadings.Params;

            ListMode = GH_ValueListMode.CheckList;

            base.ListItems.Clear();
            base.ListItems.Add(new GH_ValueListItem("Choose type with right-click", "-1"));
        }

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            Menu_AppendSeparator(menu);
            foreach (AutoSettingsMode mode in Enum.GetValues(typeof(AutoSettingsMode)))
            {
                var desc = mode.GetAttributeOfType<DescriptionWithExtensionAttribute>();
                Menu_AppendItem(menu,
                                desc.Description,
                                (s, e) =>
                                {
                                    _mode = mode;
                                    RefreshAvailableAttributes(desc);
                                },
                                enabled: true,
                                _mode == mode);
            }

            Menu_AppendSeparator(menu);
        }

        private void RefreshAvailableAttributes(DescriptionWithExtensionAttribute descriptionWithExtension)
        {
            NickName = $"{descriptionWithExtension.Description} settings";
            ListItems.Clear();

            var files = new Tekla.Structures.TeklaStructuresFiles(ModelInteractor.ModelPath())
                .GetMultiDirectoryFileList(descriptionWithExtension.Extension);

            files.Sort(new NaturalStringComparer());
            if (files.Contains("standard"))
            {
                files.Remove("standard");
                files.Insert(0, "standard");
            }

            foreach (var file in files)
                ListItems.Add(new GH_ValueListItem(file, $"\"{file}\""));

            ExpireSolution(recompute: true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(nameof(AutoSettingsMode), (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(nameof(AutoSettingsMode), ref serializedInt);
            _mode = (AutoSettingsMode)serializedInt;
            return base.Read(reader);
        }

        enum AutoSettingsMode
        {
            [DescriptionWithExtension("A drawing", "ad")]
            DrA,
            [DescriptionWithExtension("Arc", "gar")]
            DrArc,
            [DescriptionWithExtension("Associative note", "note")]
            DrAssociativeNote,
            [DescriptionWithExtension("Bolt", "sc")]
            DrBolt,
            [DescriptionWithExtension("Bolt mark", "sm")]
            DrBoltMark,
            [DescriptionWithExtension("Circle", "gci")]
            DrCircle,
            [DescriptionWithExtension("CU drawing", "cud")]
            DrCU,
            [DescriptionWithExtension("Detail mark", "detail")]
            DrDetailMark,
            [DescriptionWithExtension("Dimension", "dim")]
            DrDimension,
            [DescriptionWithExtension("DWG/DXF", "fdg")]
            DrDwgDxf,
            [DescriptionWithExtension("GA drawing", "gd")]
            DrGA,
            [DescriptionWithExtension("Hyperlink", "fhl")]
            DrHyperlink,
            [DescriptionWithExtension("Image", "idf")]
            DrImage,
            [DescriptionWithExtension("Level mark", "lev")]
            DrLevelMark,
            [DescriptionWithExtension("Line", "gln")]
            DrLine,
            [DescriptionWithExtension("Mesh", "drmsh")]
            DrMesh,
            [DescriptionWithExtension("Part", "dprt")]
            DrPart,
            [DescriptionWithExtension("Part mark", "pm")]
            DrPartMark,
            [DescriptionWithExtension("Polygon", "gpg")]
            DrPolygon,
            [DescriptionWithExtension("Polyline", "gpl")]
            DrPolyline,
            [DescriptionWithExtension("Rebar", "drbr")]
            DrRebar,
            [DescriptionWithExtension("Rebar dimension", "rdim")]
            DrRebarDimension,
            [DescriptionWithExtension("Rebar mark", "rm")]
            DrRebarMark,
            [DescriptionWithExtension("Rectangle", "grt")]
            DrRectangle,
            [DescriptionWithExtension("Revision mark", "rev")]
            DrRevisionMark,
            [DescriptionWithExtension("Rich text", "fas")]
            DrRichText,
            [DescriptionWithExtension("Section mark", "cs")]
            DrSectionMark,
            [DescriptionWithExtension("Selection filter", "SObjGrp")]
            MoSelectionFilter,
            [DescriptionWithExtension("Symbol", "sbl")]
            DrSymbol,
            [DescriptionWithExtension("Text", "drtxt")]
            DrText,
            [DescriptionWithExtension("View", "vi")]
            DrView,
            [DescriptionWithExtension("W drawing", "wd")]
            DrW,
            [DescriptionWithExtension("Weld", "welo")]
            DrWeld,
            [DescriptionWithExtension("Weld mark", "wel")]
            DrWeldMark,
        }

        class DescriptionWithExtensionAttribute : DescriptionAttribute
        {
            public string Extension { get; }

            public DescriptionWithExtensionAttribute(string description, string extraInfo) : base(description)
            {
                Extension = extraInfo;
            }
        }
    }
}
