using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GTDrawingLink.Tools;
using static GTDrawingLink.Tools.ComponentInfos;

namespace GTDrawingLink.Components.Params.ValueLists
{
    public class ViewingDirectionValueList : GH_ValueList
    {
        public override Guid ComponentGuid => new Guid("542AFB31-C81E-4CEB-9543-E757FD5B31D0");
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.ViewingDirection;

        public ViewingDirectionValueList() : base()
        {
            Name = "Viewing Direction";
            Description = "Viewing Direction Vector";

            Category = VersionSpecificConstants.TabHeading;
            SubCategory = PanelHeadings.Params;

            PopulateAvailableTypes();
        }

        private void PopulateAvailableTypes()
        {
            ListItems.Clear();
            ListItems.Add(new GH_ValueListItem("From Left", "{1,0,0}"));
            ListItems.Add(new GH_ValueListItem("From Right", "{-1,0,0}"));
            ListItems.Add(new GH_ValueListItem("From Bottom", "{0,1,0}"));
            ListItems.Add(new GH_ValueListItem("From Top", "{0,-1,0}"));

            SelectItem(0);
        }
    }

}
