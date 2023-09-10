using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GTDrawingLink.Tools;
using static GTDrawingLink.Tools.ComponentInfos;

namespace GTDrawingLink.Components.ValueLists
{
    public class DrawingColorsValueList : GH_ValueList
    {
        public override Guid ComponentGuid => new Guid("A4CD1584-4859-4B27-9AAD-16BE189FF6AB");
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.DrawingColors;

        public DrawingColorsValueList() : base()
        {
            Name = "Drawing Colors";
            Description = "Value List with Tekla Drawing Colors";

            Category = VersionSpecificConstants.TabHeading;
            SubCategory = PanelHeadings.Params;

            PopulateAvailableColors();
        }

        private void PopulateAvailableColors()
        {
            ListItems.Clear();
            ListItems.Add(new GH_ValueListItem("Invisible", "152"));
            ListItems.Add(new GH_ValueListItem("Black", "153"));
            ListItems.Add(new GH_ValueListItem("Brown", "154"));
            ListItems.Add(new GH_ValueListItem("Green", "155"));
            ListItems.Add(new GH_ValueListItem("Dark blue", "156"));
            ListItems.Add(new GH_ValueListItem("Forest green", "157"));
            ListItems.Add(new GH_ValueListItem("Orange", "158"));
            ListItems.Add(new GH_ValueListItem("Gray", "159"));
            ListItems.Add(new GH_ValueListItem("Red", "160"));
            ListItems.Add(new GH_ValueListItem("Green", "161"));
            ListItems.Add(new GH_ValueListItem("Blue", "162"));
            ListItems.Add(new GH_ValueListItem("Cyan", "163"));
            ListItems.Add(new GH_ValueListItem("Yellow", "164"));
            ListItems.Add(new GH_ValueListItem("Magenta", "165"));
            ListItems.Add(new GH_ValueListItem("Gray30", "130"));
            ListItems.Add(new GH_ValueListItem("Gray50", "131"));
            ListItems.Add(new GH_ValueListItem("Gray70", "132"));
            ListItems.Add(new GH_ValueListItem("Gray90", "133"));

            SelectItem(1);
        }
    }
}
