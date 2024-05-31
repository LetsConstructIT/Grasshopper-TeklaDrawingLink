using Grasshopper.Kernel;

namespace GTDrawingLink.Tools
{
    public static class ComponentInfos
    {
        public static class PanelHeadings
        {
            public static readonly string Params = "             Params";
            public static readonly string Drawing = "            Drawing";
            public static readonly string View = "          View";
            public static readonly string DrawingParts = "         Parts";
            public static readonly string Attributes = "        Attributes";
            public static readonly string Geometry = "       Geometry";
            public static readonly string Misc = "      Misc";
            public static readonly string Udas = "    UDAs";
            public static readonly string Plugins = "   Plugins";
            public static readonly string Dimensions = "  Dimensions";
            public static readonly string Annotations = " Annotations";
            public static readonly string Modify = "Modify";
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
            SubCategory = PanelHeadings.Annotations
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

        public static readonly GH_InstanceDescription CreateCurvedSectionViewComponent = new GH_InstanceDescription
        {
            Name = "Curved Section View",
            NickName = "CurvedSection",
            Description = "Create curved section view",
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

        public static readonly GH_InstanceDescription GetViewsInDrawingComponent = new GH_InstanceDescription
        {
            Name = "Views in Drawing",
            NickName = "Views",
            Description = "Get views in drawing. Right click to change mode from the top most to all.",
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

        public static readonly GH_InstanceDescription ModifyDrawingPropertiesComponent = new GH_InstanceDescription
        {
            Name = "Drawing Properties",
            NickName = "Prop",
            Description = "Get or set drawing properties",
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

        public static readonly GH_InstanceDescription CreateGADrawingComponent = new GH_InstanceDescription
        {
            Name = "Create GA Drawing",
            NickName = "GADrawing",
            Description = "Create GA drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetSelectedDrawingsOnListComponent = new GH_InstanceDescription
        {
            Name = "Selected Drawings",
            NickName = "DrawingFromDrList",
            Description = "Gets the selected drawings from drawing list",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetDrawingsComponent = new GH_InstanceDescription
        {
            Name = "All Drawings",
            NickName = "AllDrawings",
            Description = "Gets all drawings",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
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
            Description = "Gets center of gravity of model or drawing object in global coordinate system",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
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
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription GetSelectedComponent = new GH_InstanceDescription
        {
            Name = "Selected Objects",
            NickName = "Selected",
            Description = "Get selected drawing objects",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription SetDrawingUDAComponent = new GH_InstanceDescription
        {
            Name = "Set Drawing UDAs",
            NickName = "USet",
            Description = "Set User-defined attributes for any drawing object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Udas
        };

        public static readonly GH_InstanceDescription GetDrawingUDAValueComponent = new GH_InstanceDescription
        {
            Name = "Get Drawing UDA Value",
            NickName = "UGet",
            Description = "Get the value of a User-defined attribute from any drawing object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Udas
        };

        public static readonly GH_InstanceDescription GetDrawingAllUDAsComponent = new GH_InstanceDescription
        {
            Name = "Get All Drawing UDAs",
            NickName = "UGet",
            Description = "Get all User-defined attributes from any drawing object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Udas
        };

        public static readonly GH_InstanceDescription PickerInputTypeComponent = new GH_InstanceDescription
        {
            Name = "Input Type",
            NickName = "IType",
            Description = "Creates specific input types for Tekla drawing plugins. Right-click to choose type.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Plugins
        };

        public static readonly GH_InstanceDescription PickerInputComponent = new GH_InstanceDescription
        {
            Name = "Picker Input",
            NickName = "Input",
            Description = "Creates input for Tekla drawing plugins from multiple picker input types.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Plugins
        };

        public static readonly GH_InstanceDescription CreatePluginComponent = new GH_InstanceDescription
        {
            Name = "Plugin",
            NickName = "Plugin",
            Description = "Creates a drawing component in Tekla.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Plugins
        };

        public static readonly GH_InstanceDescription StraightDimensionSetAttributesComponent = new GH_InstanceDescription
        {
            Name = "Dimension Line Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a dimension line attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription DeconstructDimensionSetComponent = new GH_InstanceDescription
        {
            Name = "Deconstruct Dimension Line",
            NickName = "DeconstructDim",
            Description = "Get data from Tekla dimension line.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription CreateStraightDimensionSetComponent = new GH_InstanceDescription
        {
            Name = "Dimension Line",
            NickName = "DimLine",
            Description = "Create Tekla dimension line.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription CreateDimensionLinkComponent = new GH_InstanceDescription
        {
            Name = "Dimension Link",
            NickName = "DimLink",
            Description = "Create Tekla dimension link.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription GetExtremePointsComponent = new GH_InstanceDescription
        {
            Name = "Extreme Points",
            NickName = "ExtPts",
            Description = "Get planar extreme points",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription GetPartLinesComponent = new GH_InstanceDescription
        {
            Name = "Part Lines",
            NickName = "PartLines",
            Description = "Get Part lines",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription GetCustomPartPointsComponent = new GH_InstanceDescription
        {
            Name = "Custom Part Points",
            NickName = "CmPrtPts",
            Description = "Get Custom Part points",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription CreateAngleDimensionComponent = new GH_InstanceDescription
        {
            Name = "Angle Dimension",
            NickName = "AngDim",
            Description = "Create Tekla angle dimension.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription ObjectMatchesToFilterComponent = new GH_InstanceDescription
        {
            Name = "Object Match",
            NickName = "Match",
            Description = "Checks whether the Tekla model object matches to the criteria in the given filter.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription GroupObjectsComponent = new GH_InstanceDescription
        {
            Name = "Group Objects",
            NickName = "G",
            Description = "Group Tekla model objects by specified criteria (right-click to choose)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };
        public static readonly GH_InstanceDescription CreateTextComponent = new GH_InstanceDescription
        {
            Name = "Text",
            NickName = "Txt",
            Description = "Create Tekla text.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };
        public static readonly GH_InstanceDescription TextAttributesComponent = new GH_InstanceDescription
        {
            Name = "Text Attributes",
            NickName = "TxtAttr",
            Description = "Create Tekla text attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };
        public static readonly GH_InstanceDescription FontAttributesComponent = new GH_InstanceDescription
        {
            Name = "Font Attributes",
            NickName = "FAttr",
            Description = "Create font attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };
        public static readonly GH_InstanceDescription ArrowAttributesComponent = new GH_InstanceDescription
        {
            Name = "Arrow Attributes",
            NickName = "AAttr",
            Description = "Create arrow attribute.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };
        public static readonly GH_InstanceDescription LineAttributesComponent = new GH_InstanceDescription
        {
            Name = "Line Attributes",
            NickName = "LAttr",
            Description = "Create line attribute.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };
        public static readonly GH_InstanceDescription PolylineAttributesComponent = new GH_InstanceDescription
        {
            Name = "Polyline Attributes",
            NickName = "PLAttr",
            Description = "Create polyline attribute.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription DeleteDrawingObjectsComponent = new GH_InstanceDescription
        {
            Name = "Delete Objects",
            NickName = "D",
            Description = "Delete Tekla drawing objects",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription GetObjectsFromViewComponent = new GH_InstanceDescription
        {
            Name = "Objects from View",
            NickName = "G",
            Description = "Get Tekla drawing objects from View",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription RefreshViewComponent = new GH_InstanceDescription
        {
            Name = "Refresh View",
            NickName = "R",
            Description = "Refresh Tekla Drawing View",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription GetModelViewsComponent = new GH_InstanceDescription
        {
            Name = "Model Views",
            NickName = "MVs",
            Description = "Get Tekla model views (right-click to filter only visible views)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription ConstructModelViewComponent = new GH_InstanceDescription
        {
            Name = "Model View",
            NickName = "MV",
            Description = "Construct Tekla model view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription DeconstructModelViewComponent = new GH_InstanceDescription
        {
            Name = "Deconstruct Model View",
            NickName = "DMV",
            Description = "Deconstruct Tekla model view",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription CreateModelViewComponent = new GH_InstanceDescription
        {
            Name = "Model View",
            NickName = "MV",
            Description = "Create Tekla model view in the drawing area",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };

        public static readonly GH_InstanceDescription ReinforcementMeshAttributesComponent = new GH_InstanceDescription
        {
            Name = "Mesh Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a reinforcement mesh attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ReinforcementAttributesComponent = new GH_InstanceDescription
        {
            Name = "Rebar Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a reinforcement attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ModifyRebarComponent = new GH_InstanceDescription
        {
            Name = "Modify Rebar",
            NickName = "MRebar",
            Description = "Modifies a reinforcement (single/group/strand/set) in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription ModifyMeshComponent = new GH_InstanceDescription
        {
            Name = "Modify Mesh",
            NickName = "MMesh",
            Description = "Modifies a mesh in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription FrameAttributesComponent = new GH_InstanceDescription
        {
            Name = "Frame Attributes",
            NickName = "FAttr",
            Description = "Create frame attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription SymbolAttributesComponent = new GH_InstanceDescription
        {
            Name = "Symbol Attributes",
            NickName = "SAttr",
            Description = "Create symbol attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription SymbolSelectionComponent = new GH_InstanceDescription
        {
            Name = "Symbol Selection",
            NickName = "SSel",
            Description = "Create symbol selection (file and number).",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };


        public static readonly GH_InstanceDescription CreateSymbolComponent = new GH_InstanceDescription
        {
            Name = "Symbol",
            NickName = "Sym",
            Description = "Create Tekla Drawing Symbol.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription PartAttributesComponent = new GH_InstanceDescription
        {
            Name = "Part Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a part attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription DeleteDrawingComponent = new GH_InstanceDescription
        {
            Name = "Delete Drawing",
            NickName = "Del",
            Description = "Delete Tekla drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription GetDrawingsFromModelObjectComponent = new GH_InstanceDescription
        {
            Name = "Drawings From Model Object",
            NickName = "Dr",
            Description = "Get Tekla drawings from source model object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription OrderStraightDimensionSetComponent = new GH_InstanceDescription
        {
            Name = "Order Dim Lines",
            NickName = "ODim",
            Description = "Orders Tekla dimension line by specified distance.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription BoltAttributesComponent = new GH_InstanceDescription
        {
            Name = "Bolt Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a bolt attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ModifyBoltComponent = new GH_InstanceDescription
        {
            Name = "Modify Bolt",
            NickName = "MBolt",
            Description = "Modifies a drawing bolt in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription CreateAssociativeNoteComponent = new GH_InstanceDescription
        {
            Name = "Associative Note",
            NickName = "A",
            Description = "Create Associative Note",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CreateMarkComponent = new GH_InstanceDescription
        {
            Name = "Mark",
            NickName = "M",
            Description = "Create Part/Rebar/Weld/Bolt Mark by 'macroing'",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription WeldAttributesComponent = new GH_InstanceDescription
        {
            Name = "Weld Attributes",
            NickName = "Attributes",
            Description = "Creates/modifies a weld attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription ModifyWeldComponent = new GH_InstanceDescription
        {
            Name = "Modify Weld",
            NickName = "MWeld",
            Description = "Modifies a drawing weld in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription GetBoltPropertiesComponent = new GH_InstanceDescription
        {
            Name = "Bolt Properties",
            NickName = "BProps",
            Description = "Returns Tekla model bolt properties ",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription GetReinforcementPropertiesComponent = new GH_InstanceDescription
        {
            Name = "Reinforcement Properties",
            NickName = "RProps",
            Description = "Returns Tekla model reinforcement properties ",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription SelectModelObjectComponent = new GH_InstanceDescription
        {
            Name = "Select Model Object",
            NickName = "SelMO",
            Description = "Select object in model space",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription GetSelectedModelObjectComponent = new GH_InstanceDescription
        {
            Name = "Selected Model Objects",
            NickName = "SelectedMO",
            Description = "Get selected model objects",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription PerformNumberingComponent = new GH_InstanceDescription
        {
            Name = "Perform Numbering",
            NickName = "Numb",
            Description = "Performs numbering in Tekla model area (right-click to numbering type)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription MarkAttributesComponent = new GH_InstanceDescription
        {
            Name = "Mark Attributes",
            NickName = "MAttr",
            Description = "Create Tekla Mark Attributes.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };


        public static readonly GH_InstanceDescription GetRelatedObjectsComponent = new GH_InstanceDescription
        {
            Name = "Related Objects",
            NickName = "RelatedObjects",
            Description = "Get related drawing objects",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription ConvertModelToDrawingObjectComponent = new GH_InstanceDescription
        {
            Name = "Model to Drawing Object",
            NickName = "MtoD",
            Description = "Converts the Tekla Structures model object to associated drawing object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.DrawingParts
        };

        public static readonly GH_InstanceDescription CreateLineComponent = new GH_InstanceDescription
        {
            Name = "Line",
            NickName = "L",
            Description = "Draw Line in Tekla Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CreatePolylineComponent = new GH_InstanceDescription
        {
            Name = "Polyline",
            NickName = "PL",
            Description = "Draw Polyline in Tekla Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription EmbeddedObjectAttributesComponent = new GH_InstanceDescription
        {
            Name = "DWG Attributes",
            NickName = "DAttr",
            Description = "Create DWG/DXF attribute.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };


        public static readonly GH_InstanceDescription CreateEmbeddedObjectComponent = new GH_InstanceDescription
        {
            Name = "DWG",
            NickName = "D",
            Description = "Insert DWG/DXF as a link",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CreateDrawingLibraryComponent = new GH_InstanceDescription
        {
            Name = "2D Library",
            NickName = "2D",
            Description = "Insert 2D Drawing Library detail",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription LoopStartComponent = new GH_InstanceDescription
        {
            Name = "Loop Start",
            NickName = "S",
            Description = "Start of foreach loop",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription LoopEndComponent = new GH_InstanceDescription
        {
            Name = "Loop End",
            NickName = "E",
            Description = "End of foreach loop",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };
        public static readonly GH_InstanceDescription RotateViewComponent = new GH_InstanceDescription
        {
            Name = "Rotate View",
            NickName = "R",
            Description = "Rotate drawing view on the drawing plane",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.View
        };
        public static readonly GH_InstanceDescription FindVisibleEdgesComponent = new GH_InstanceDescription
        {
            Name = "Find Visible Edges",
            NickName = "F",
            Description = "Find edges visible from side",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription BrepProjectionBorderComponent = new GH_InstanceDescription
        {
            Name = "Brep Projection Border",
            NickName = "Border",
            Description = "Gets the outer border and all inner borders of brep projection",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription SearchUsingKeyComponent = new GH_InstanceDescription
        {
            Name = "Search using Key",
            NickName = "Search",
            Description = "Find branch with provided key",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription DimensionBoxComponent = new GH_InstanceDescription
        {
            Name = "Dimension Box",
            NickName = "DimBox",
            Description = "Calculates bounding box of initial rectangle and provided planar geometries",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription SortByVectorComponent = new GH_InstanceDescription
        {
            Name = "Sort by Vector",
            NickName = "SortVector",
            Description = "Arrange geometry objects according to their projection on plane created from specified vector and Z-dir",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription SortByKeyComponent = new GH_InstanceDescription
        {
            Name = "Sort by Key",
            NickName = "SortKey",
            Description = "Arrange objects according to provided order",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription BakeToTeklaComponent = new GH_InstanceDescription
        {
            Name = "Bake to Tekla",
            NickName = "Bake",
            Description = "Break live link with Tekla for input component",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription SimpleOrientComponent = new GH_InstanceDescription
        {
            Name = "Simple Orient",
            NickName = "Orient",
            Description = "Orient a tree of objects to specified coordinate system. Path convention handles multiple input coordinate systems.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription CreateWDrawingComponent = new GH_InstanceDescription
        {
            Name = "Create W Drawing",
            NickName = "WDrawing",
            Description = "Create single part drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription TeklaIndexComponent = new GH_InstanceDescription
        {
            Name = "Tekla Index",
            NickName = "TIndex",
            Description = "Find the occurences of a specific Tekla object in a set.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Misc
        };

        public static readonly GH_InstanceDescription GetGridPropertiesComponent = new GH_InstanceDescription
        {
            Name = "Grid Properties",
            NickName = "GProp",
            Description = "Deconstruct Tekla Grid object",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription GetEditModeComponent = new GH_InstanceDescription
        {
            Name = "Edit Mode",
            NickName = "EMode",
            Description = "Tekla edit mode (model or drawing)",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Drawing
        };

        public static readonly GH_InstanceDescription DeconstructPluginComponent = new GH_InstanceDescription
        {
            Name = "Deconstruct Plugin",
            NickName = "DP",
            Description = "Deconstruct Tekla Drawing Plugin",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Plugins
        };

        public static readonly GH_InstanceDescription CreateDetailMarkComponent = new GH_InstanceDescription
        {
            Name = "Detail Mark",
            NickName = "DM",
            Description = "Create detail mark",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CreateSectionMarkComponent = new GH_InstanceDescription
        {
            Name = "Section Mark",
            NickName = "SM",
            Description = "Create section mark",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription SplitGeometryComponent = new GH_InstanceDescription
        {
            Name = "Split Geometry",
            NickName = "Split",
            Description = "Split the provided geometry list based on the location associated with a specified point and direction.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Geometry
        };

        public static readonly GH_InstanceDescription PlacingBaseComponent = new GH_InstanceDescription
        {
            Name = "Placing Type",
            NickName = "Placing",
            Description = "Placing type for Tekla drawing Marks/Texts",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription CreatePolygonComponent = new GH_InstanceDescription
        {
            Name = "Polygon",
            NickName = "POLY",
            Description = "Draw Polygon in Tekla Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription PolygonAttributesComponent = new GH_InstanceDescription
        {
            Name = "Polygon Attributes",
            NickName = "POLYAttr",
            Description = "Create polygon attribute.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription DeconstructTextComponent = new GH_InstanceDescription
        {
            Name = "Deconstruct Text",
            NickName = "DeconText",
            Description = "Get the data from Tekla Text object.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription DeconstructMarkComponent = new GH_InstanceDescription
        {
            Name = "Deconstruct Mark",
            NickName = "DeconMark",
            Description = "Get the data from Tekla Mark object.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription ModifyTextComponent = new GH_InstanceDescription
        {
            Name = "Modify Text",
            NickName = "MText",
            Description = "Modifies a drawing text in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription ModifyMarkComponent = new GH_InstanceDescription
        {
            Name = "Modify Mark",
            NickName = "MMark",
            Description = "Modifies a drawing mark in Tekla Structures",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Modify
        };

        public static readonly GH_InstanceDescription CreateCircleComponent = new GH_InstanceDescription
        {
            Name = "Circle",
            NickName = "C",
            Description = "Draw Circle in Tekla Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CircleAttributesComponent = new GH_InstanceDescription
        {
            Name = "Circle Attributes",
            NickName = "CAttr",
            Description = "Create circle attributes",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription CreateArcComponent = new GH_InstanceDescription
        {
            Name = "Arc",
            NickName = "A",
            Description = "Draw Arc in Tekla Drawing",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription ArcAttributesComponent = new GH_InstanceDescription
        {
            Name = "Arc Attributes",
            NickName = "AAttr",
            Description = "Create arc attributes",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };

        public static readonly GH_InstanceDescription TextFileAttributesComponent = new GH_InstanceDescription
        {
            Name = "Rich Text Attributes",
            NickName = "RAttr",
            Description = "Create rich text attributes",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Attributes
        };
        public static readonly GH_InstanceDescription CreateTextFileComponent = new GH_InstanceDescription
        {
            Name = "RichText",
            NickName = "RTxt",
            Description = "Create Tekla rich text.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Annotations
        };

        public static readonly GH_InstanceDescription CreateRadialDimensionComponent = new GH_InstanceDescription
        {
            Name = "Radial Dimension",
            NickName = "RadDim",
            Description = "Create Tekla radial dimension.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };

        public static readonly GH_InstanceDescription CreateCurvedDimensionSetComponent = new GH_InstanceDescription
        {
            Name = "Curved Dimension",
            NickName = "CurDim",
            Description = "Create Tekla curved dimension.",
            Category = VersionSpecificConstants.TabHeading,
            SubCategory = PanelHeadings.Dimensions
        };
    }
}
