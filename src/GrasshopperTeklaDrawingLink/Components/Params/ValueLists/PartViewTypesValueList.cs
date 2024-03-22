using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GTDrawingLink.Tools;
using static GTDrawingLink.Tools.ComponentInfos;

namespace GTDrawingLink.Components.Params.ValueLists
{
    public class PartViewTypesValueList : GH_ValueList
    {
        public override Guid ComponentGuid => new Guid("68FB5C8A-F9FD-47CA-B216-2F93F62F90DB");
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.PartViewTypes;

        public PartViewTypesValueList() : base()
        {
            Name = "Part View Types";
            Description = "Tekla part view types as value list";

            Category = VersionSpecificConstants.TabHeading;
            SubCategory = PanelHeadings.Params;

            PopulateAvailableTypes();
        }

        private void PopulateAvailableTypes()
        {
            ListItems.Clear();
            ListItems.Add(new GH_ValueListItem("Front", "\"FrontView\""));
            ListItems.Add(new GH_ValueListItem("Top", "\"TopView\""));
            ListItems.Add(new GH_ValueListItem("Back", "\"BackView\""));
            ListItems.Add(new GH_ValueListItem("Bottom", "\"BottomView\""));
            ListItems.Add(new GH_ValueListItem("3d", "\"_3DView\""));

            SelectItem(0);
        }
    }
}
