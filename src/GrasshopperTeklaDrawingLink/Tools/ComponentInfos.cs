using Grasshopper.Kernel;

namespace GTDrawingLink.Tools
{
    public static class ComponentInfos
    {
        public static class PanelHeadings
        {
            public static readonly string Params =        "        Params";
            public static readonly string Drawing =       "       Drawing";
            public static readonly string DrawingList =   "      Drawing List";
            public static readonly string View =          "     View";
            public static readonly string DrawingParts =  "    Parts";
            public static readonly string Attributes =    "   Attributes";
            public static readonly string Geometry      = "  Geometry";
            public static readonly string Misc =          " Misc";
            public static readonly string Marks =         "Marks";
        }

        public static readonly GH_InstanceDescription DrawingObjectParam = new GH_InstanceDescription
        {
            Name = "Drawing Object",
            NickName = "DO",
            Description = "Reference any drawing object from Tekla Structures into Grasshopper (right-click this component for the options to pick one or several objects in Tekla Structures)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Params
        };

        public static readonly GH_InstanceDescription DrawingPartParam = new GH_InstanceDescription
        {
            Name = "Drawing Part",
            NickName = "DrPrt",
            Description = "Reference a drawing part from Tekla Structures into Grasshopper (right-click this component for the options to pick one or several objects in Tekla Structures)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Params
        };

        public static readonly GH_InstanceDescription TeklaDrawingPointParam = new GH_InstanceDescription
        {
            Name = "Drawing Point",
            NickName = "DrPt",
            Description = "Set a point in Tekla Structures drawing to be used in Grasshopper (right-click this component for the options to pick one or several objects in Tekla Structures)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Params
        };        

        public static readonly GH_InstanceDescription ConvertDrawingToModelObjectComponent = new GH_InstanceDescription
        {
            Name = "Drawing to Model Object",
            NickName = "DtoM",
            Description = "Converts the Tekla Structures drawing object to associated model object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription GetViewFromDrawingObjectComponent = new GH_InstanceDescription
        {
            Name = "Father View",
            NickName = "ViewFromDrObj",
            Description = "Gets the view where the drawing object is",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription GetActiveDrawingComponent = new GH_InstanceDescription
        {
            Name = "Active Drawing",
            NickName = "ActiveDrawing",
            Description = "Get currently opened Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription CreateLevelMarkComponent = new GH_InstanceDescription
        {
            Name = "Level Mark",
            NickName = "LevelMark",
            Description = "Creates Level Mark at the specified point",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Marks
        };

        public static readonly GH_InstanceDescription CreatePartViewComponent = new GH_InstanceDescription
        {
            Name = "Part View",
            NickName = "CreatePartView",
            Description = "Create part view with specified type",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription CreateDetailViewComponent = new GH_InstanceDescription
        {
            Name = "Detail View",
            NickName = "CreateDetailView",
            Description = "Create detail view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription CreateSectionViewComponent = new GH_InstanceDescription
        {
            Name = "Section View",
            NickName = "CreateSectionViewComponent",
            Description = "Create section view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };        

        public static readonly GH_InstanceDescription GetViewFrameGeometryComponent = new GH_InstanceDescription
        {
            Name = "View frame geometry",
            NickName = "GetViewGeometry",
            Description = "Get frame geometry of specified view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription GetViewsAtDrawingComponent = new GH_InstanceDescription
        {
            Name = "Views at Drawing",
            NickName = "GetViewsAtDrawing",
            Description = "Get views at current drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetViewPropertiesComponent = new GH_InstanceDescription
        {
            Name = "View properties",
            NickName = "ViewProperties",
            Description = "Get view main properties (name, type)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };


        public static readonly GH_InstanceDescription GetRelatedViewsComponent = new GH_InstanceDescription
        {
            Name = "Related views",
            NickName = "RelatedViews",
            Description = "Get section/detail views related to input view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };        

        public static readonly GH_InstanceDescription MoveViewComponent = new GH_InstanceDescription
        {
            Name = "Move View",
            NickName = "MoveViewByVector",
            Description = "Move View by vector",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription CloseDrawingComponent = new GH_InstanceDescription
        {
            Name = "Close Drawing",
            NickName = "CloseDrawing",
            Description = "Close drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription OpenDrawingComponent = new GH_InstanceDescription
        {
            Name = "Open Drawing",
            NickName = "OpenDrawing",
            Description = "Open drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetDrawingSizeComponent = new GH_InstanceDescription
        {
            Name = "Drawing Size",
            NickName = "DrawingSize",
            Description = "Get drawing size",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetDrawingSourceObjectComponent = new GH_InstanceDescription
        {
            Name = "Drawing Source",
            NickName = "DrawingSource",
            Description = "Get drawing source",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription CreateCUDrawingComponent = new GH_InstanceDescription
        {
            Name = "Create CU Drawing",
            NickName = "CUDrawing",
            Description = "Create cast unit drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription CreateADrawingComponent = new GH_InstanceDescription
        {
            Name = "Create A Drawing",
            NickName = "ADrawing",
            Description = "Create assembly drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };        

        public static readonly GH_InstanceDescription GetSelectedDrawingsOnListComponent = new GH_InstanceDescription
        {
            Name = "Selected Drawings on List",
            NickName = "DrawingFromDrList",
            Description = "Gets the selected drawings from drawing list",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingList
        };

        public static readonly GH_InstanceDescription GetDrawingsComponent = new GH_InstanceDescription
        {
            Name = "All Drawings",
            NickName = "AllDrawings",
            Description = "Gets all drawings",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingList
        };

        public static readonly GH_InstanceDescription TransformPointToViewCS = new GH_InstanceDescription
        {
            Name = "Point to View",
            NickName = "Point2View",
            Description = "Transform Point coordinates from global to view coordinate system",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription TransformPointToGlobalCS = new GH_InstanceDescription
        {
            Name = "Point to Global",
            NickName = "Point2Global",
            Description = "Transform Point coordinates from local to global coordinate system",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription TransformPointToLocalCS = new GH_InstanceDescription
        {
            Name = "Point to Local",
            NickName = "Point2Local",
            Description = "Transform Point coordinates from global to local coordinate system",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription SelectDrawingObjectComponent = new GH_InstanceDescription
        {
            Name = "Select Object",
            NickName = "SelectObject",
            Description = "Select object in drawing space",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription RunMacroComponent = new GH_InstanceDescription
        {
            Name = "Run Macro",
            NickName = "Macro",
            Description = "Run macro (right-click this component for choosing type of macro: model / drawing / dynamic. Dynamic means creating macro ad-hoc from provided macro content)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription GetCOGComponent = new GH_InstanceDescription
        {
            Name = "Get COG",
            NickName = "COG",
            Description = "Gets center of gravity of model or drawing object in current coordinate system",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription LineTypeAttributesComponent = new GH_InstanceDescription
        {
            Name = "Line Attributes",
            NickName = "LineType",
            Description = "Line type attributes",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ModelObjectHatchAttributesComponent = new GH_InstanceDescription
        {
            Name = "Hatch Attributes",
            NickName = "HatchType",
            Description = "Hatch type attributes",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ModifyPartComponent = new GH_InstanceDescription
        {
            Name = "Modify Part",
            NickName = "MPart",
            Description = "Modifies a drawing part in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription GetSelectedComponent = new GH_InstanceDescription
        {
            Name = "Selected Objects",
            NickName = "Selected",
            Description = "Get selected objects",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };
    }
}
