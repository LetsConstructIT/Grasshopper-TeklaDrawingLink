using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GTDrawingLink.Tools;
using static GTDrawingLink.Tools.ComponentInfos;

namespace GTDrawingLink.Components.Params.ValueLists
{
    public class DrawingObjectTypesValueList : GH_ValueList
    {
        public override Guid ComponentGuid => new Guid("DC2512F2-0A83-4EE7-A6BF-9754B5CFC0CA");
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.DrawingObjectTypes;

        public DrawingObjectTypesValueList() : base()
        {
            Name = "Drawing Object Types";
            Description = "Tekla Drawing object types as value list";

            Category = VersionSpecificConstants.TabHeading;
            SubCategory = PanelHeadings.Params;

            ListMode = GH_ValueListMode.DropDown;
            PopulateAvailableTypes();
        }

        private void PopulateAvailableTypes()
        {
            ListItems.Clear();
            ListItems.Add(new GH_ValueListItem("Arc", "\"Arc\""));
            ListItems.Add(new GH_ValueListItem("Bolt", "\"Bolt\""));
            ListItems.Add(new GH_ValueListItem("Circle", "\"Circle\""));
            ListItems.Add(new GH_ValueListItem("Cloud", "\"Cloud\""));
            ListItems.Add(new GH_ValueListItem("Connection", "\"Connection\""));
            ListItems.Add(new GH_ValueListItem("Container View", "\"Container View\""));
            ListItems.Add(new GH_ValueListItem("Detail Mark", "\"Detail Mark\""));
            ListItems.Add(new GH_ValueListItem("Dwg Object", "\"Dwg Object\""));
            ListItems.Add(new GH_ValueListItem("Grid", "\"Grid\""));
            ListItems.Add(new GH_ValueListItem("Hyper Link", "\"Hyper Link\""));
            ListItems.Add(new GH_ValueListItem("Leader Line", "\"Leader Line\""));
            ListItems.Add(new GH_ValueListItem("Level Mark", "\"Level Mark\""));
            ListItems.Add(new GH_ValueListItem("Line", "\"Line\""));
            ListItems.Add(new GH_ValueListItem("Mark", "\"Mark\""));
            ListItems.Add(new GH_ValueListItem("Mark Set", "\"Mark Set\""));
            ListItems.Add(new GH_ValueListItem("Part", "\"Part\""));
            ListItems.Add(new GH_ValueListItem("Plugin", "\"Plugin\""));
            ListItems.Add(new GH_ValueListItem("Polygon", "\"Polygon\""));
            ListItems.Add(new GH_ValueListItem("Polyline", "\"Polyline\""));
            ListItems.Add(new GH_ValueListItem("Pour Object", "\"Pour Object\""));
            ListItems.Add(new GH_ValueListItem("Radius Dimension", "\"Radius Dimension\""));
            ListItems.Add(new GH_ValueListItem("Rectangle", "\"Rectangle\""));
            ListItems.Add(new GH_ValueListItem("Reinforcement Group", "\"Reinforcement Group\""));
            ListItems.Add(new GH_ValueListItem("Reinforcement Mesh", "\"Reinforcement Mesh\""));
            ListItems.Add(new GH_ValueListItem("Reinforcement Set Group", "\"Reinforcement Set Group\""));
            ListItems.Add(new GH_ValueListItem("Reinforcement Single", "\"Reinforcement Single\""));
            ListItems.Add(new GH_ValueListItem("Reinforcement Strand", "\"Reinforcement Strand\""));
            ListItems.Add(new GH_ValueListItem("Revision Mark", "\"Revision Mark\""));
            ListItems.Add(new GH_ValueListItem("Section Mark", "\"Section Mark\""));
            ListItems.Add(new GH_ValueListItem("Straight Dimension Set", "\"Straight Dimension Set\""));
            ListItems.Add(new GH_ValueListItem("Symbol", "\"Symbol\""));
            ListItems.Add(new GH_ValueListItem("Text", "\"Text\""));
            ListItems.Add(new GH_ValueListItem("Text File", "\"Text File\""));
            ListItems.Add(new GH_ValueListItem("View", "\"View\""));
            ListItems.Add(new GH_ValueListItem("Weld", "\"Weld\""));
            ListItems.Add(new GH_ValueListItem("Weld Mark", "\"Weld Mark\""));
        }
    }
}
