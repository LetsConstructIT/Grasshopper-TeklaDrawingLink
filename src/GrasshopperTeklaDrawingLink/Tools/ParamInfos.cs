using Grasshopper.Kernel;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Tools
{
    public static class ParamInfos
    {
        public static readonly GH_InstanceDescription Drawing = new GH_InstanceDescription
        {
            Name = "Drawing",
            NickName = "D",
            Description = "Drawing"
        };

        public static readonly GH_InstanceDescription BooleanToogle = new GH_InstanceDescription
        {
            Name = "Toggle",
            NickName = "T",
            Description = "Boolean toogle for launching component"
        };

        public static readonly GH_InstanceDescription ViewBase = new GH_InstanceDescription
        {
            Name = "View Base",
            NickName = "VB",
            Description = "View base - parent class of all views, also main drawing sheet"
        };

        public static readonly GH_InstanceDescription View = new GH_InstanceDescription
        {
            Name = "View",
            NickName = "V",
            Description = "Drawing view"
        };

        public static readonly GH_InstanceDescription Mark = new GH_InstanceDescription
        {
            Name = "Mark",
            NickName = "M",
            Description = "Tekla Mark"
        };

        public static readonly GH_InstanceDescription ViewType = new GH_InstanceDescription
        {
            Name = "Type",
            NickName = "T",
            Description = "View type"
        };

        public static readonly GH_InstanceDescription DrawingType = new GH_InstanceDescription
        {
            Name = "Type",
            NickName = "T",
            Description = "Drawing type"
        };

        public static readonly GH_InstanceDescription Attributes = new GH_InstanceDescription
        {
            Name = "Attributes Name",
            NickName = "A",
            Description = "Attribute filename"
        };

        public static readonly GH_InstanceDescription Name = new GH_InstanceDescription
        {
            Name = "Name",
            NickName = "N",
            Description = "Name"
        };

        public static readonly GH_InstanceDescription Scale = new GH_InstanceDescription
        {
            Name = "Scale",
            NickName = "S",
            Description = "View scale (integer value after the colon mark)"
        };

        public static readonly GH_InstanceDescription GravityObject = new GH_InstanceDescription
        {
            Name = "Gravity object",
            NickName = "GO",
            Description = "Part/assembly from model space or part from drawing space"
        };

        public static readonly GH_InstanceDescription ModelMacro = new GH_InstanceDescription
        {
            Name = "Modeling macro",
            NickName = "Modeling",
            Description = "Macro stored in 'modeling' subdirectory of XS_MACRO_DIRECTORY"
        };

        public static readonly GH_InstanceDescription DrawingMacro = new GH_InstanceDescription
        {
            Name = "Drawing macro",
            NickName = "Drawing",
            Description = "Macro stored in 'drawings' subdirectory of XS_MACRO_DIRECTORY"
        };

        public static readonly GH_InstanceDescription DynamicMacro = new GH_InstanceDescription
        {
            Name = "Dynamic macro",
            NickName = "Dynamic",
            Description = "New macro's content, which will be saved and run"
        };

        public static readonly GH_InstanceDescription DrawingColor = new GH_InstanceDescription
        {
            Name = "Drawing Color",
            NickName = "C",
            Description = $"Drawing color:\n{EnumHelpers.EnumToString<DrawingColors>()}\nRight-click to set"
        };

        public static readonly GH_InstanceDescription DrawingHatchColor = new GH_InstanceDescription
        {
            Name = "Drawing Hatch Color",
            NickName = "C",
            Description = $"Drawing color:\n{EnumHelpers.EnumToString<DrawingHatchColors>()}\nRight-click to set"
        };

        public static readonly GH_InstanceDescription DrawingBackgroundHatchColor = new GH_InstanceDescription
        {
            Name = "Drawing Background Color",
            NickName = "BC",
            Description = $"Drawing color:\n{EnumHelpers.EnumToString<DrawingHatchColors>()}\nRight-click to set"
        };

        public static readonly GH_InstanceDescription LineType = new GH_InstanceDescription
        {
            Name = "Line Type",
            NickName = "T",
            Description = $"Line type:\n{EnumHelpers.EnumToString<LineTypesEnum>()}\nRight-click to set"
        };

        public static readonly GH_InstanceDescription LineTypeAttributes = new GH_InstanceDescription
        {
            Name = "Line attributes",
            NickName = "LAttr",
            Description = "Line type attributes"
        };

        public static readonly GH_InstanceDescription VisibleLineTypeAttributes = new GH_InstanceDescription
        {
            Name = "Visibile lines",
            NickName = "Vis",
            Description = "Part visible lines attribute"
        };

        public static readonly GH_InstanceDescription HiddenLineTypeAttributes = new GH_InstanceDescription
        {
            Name = "Hidden lines",
            NickName = "Hid",
            Description = "Part hidden lines attribute"
        };

        public static readonly GH_InstanceDescription ReferenceLineTypeAttributes = new GH_InstanceDescription
        {
            Name = "Reference lines",
            NickName = "Ref",
            Description = "Part reference lines attribute"
        };

        public static readonly GH_InstanceDescription HatchAttributes = new GH_InstanceDescription
        {
            Name = "Hatch",
            NickName = "H",
            Description = "Hatch attribute"
        };

        public static readonly GH_InstanceDescription PartFacesHatchAttributes = new GH_InstanceDescription
        {
            Name = "Part faces hatch",
            NickName = "H",
            Description = "Part faces hatch attribute"
        };

        public static readonly GH_InstanceDescription SectionHatchAttributes = new GH_InstanceDescription
        {
            Name = "Section hatch",
            NickName = "SH",
            Description = "Section hatch attribute"
        };

        public static readonly GH_InstanceDescription TeklaDrawingPart = new GH_InstanceDescription
        {
            Name = "Part",
            NickName = "P",
            Description = "Tekla Drawing Part"
        };

        public static readonly GH_InstanceDescription TeklaDatabaseObject = new GH_InstanceDescription
        {
            Name = "Drawing Object",
            NickName = "DO",
            Description = "Tekla Structures Drawing Database Object"
        };

        public static readonly GH_InstanceDescription UDAInput = new GH_InstanceDescription
        {
            Name = "UDA",
            NickName = "U",
            Description = "User-defined attributes. Add each UDA for the part on its own line (as multiline data, NOT as a list) or separate them with a semicolon.\n\nSyntax example:\nMyStringUDA \"my user text\"\nMyIntegerUDA 3\nMyFloatUDA 12.5\n\nYou can also use the 'Construct UDA' component to create your attribute input.\n\n"
        };

        public static readonly GH_InstanceDescription UDAsOutput = new GH_InstanceDescription
        {
            Name = "UDA",
            NickName = "U",
            Description = "User-defined attributes. Use the Expand UDAs component to separate the UDAs."
        };

        public static readonly GH_InstanceDescription UDAName = new GH_InstanceDescription
        {
            Name = "Name",
            NickName = "N",
            Description = "Name of user-defined attribute"
        };

        public static readonly GH_InstanceDescription UDAType = new GH_InstanceDescription
        {
            Name = "Type",
            NickName = "T",
            Description = $"Type of user-defined attribute:\n{EnumHelpers.EnumToString<AttributesIO.AttributeTypeEnum>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription UDAValue = new GH_InstanceDescription
        {
            Name = "Value",
            NickName = "V",
            Description = "Value of user-defined attribute"
        };

        public static readonly GH_InstanceDescription PickerInputValue = new GH_InstanceDescription
        {
            Name = "Input Values",
            NickName = "IValues",
            Description = "Tekla drawing objects or points/lines for plugin input."
        };

        public static readonly GH_InstanceDescription PickerInputInput = new GH_InstanceDescription
        {
            Name = "Input",
            NickName = "I",
            Description = "Tekla drawing objects or points/lines for plugin input."
        };

        public static readonly GH_InstanceDescription PickerInput = new GH_InstanceDescription
        {
            Name = "Picker Input",
            NickName = "PI",
            Description = "Tekla drawing component input for e.g. plugins that have non-standard input."
        };

        public static readonly GH_InstanceDescription Plugin = new GH_InstanceDescription
        {
            Name = "Plugin",
            NickName = "P",
            Description = "Drawing plugin"
        };

        public static readonly GH_InstanceDescription PluginName = new GH_InstanceDescription
        {
            Name = "Name",
            NickName = "N",
            Description = "Tekla drawing plugin name."
        };

        public static readonly GH_InstanceDescription PluginAttributes = new GH_InstanceDescription
        {
            Name = "Plugin Attributes",
            NickName = "PAattr",
            Description = "Drawing plugin attributes. Add each attribute for the plugin on its own line (as multiline data, NOT as a list) or separate them with a semicolon."
        };

        public static readonly GH_InstanceDescription ObjectsToSelect = new GH_InstanceDescription
        {
            Name = "Objects to select",
            NickName = "O",
            Description = "Tekla drawing objects which have to be selected before inserting the plugin."
        };

        public static readonly GH_InstanceDescription ViewAttributesLoadedByMacro = new GH_InstanceDescription
        {
            Name = "Attributes by macro",
            NickName = "M",
            Description = "Load view attributes with macro"
        };

        public static readonly GH_InstanceDescription StraightDimensionSetAttributes = new GH_InstanceDescription
        {
            Name = "Dimension Line Attributes",
            NickName = "A",
            Description = "Dimension Line Attributes"
        };

        public static readonly GH_InstanceDescription DimensionLineType = new GH_InstanceDescription
        {
            Name = "Dim Type",
            NickName = "T",
            Description = $"Type of dimension line:\n{EnumHelpers.EnumToString<DimensionSetBaseAttributes.DimensionTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription StraightDimensionSet = new GH_InstanceDescription
        {
            Name = "Dimension Line",
            NickName = "Dim",
            Description = "Dimension Line"
        };

        public static readonly GH_InstanceDescription StraightDimensionSets = new GH_InstanceDescription
        {
            Name = "Dimension Lines",
            NickName = "Dims",
            Description = "Dimension Lines"
        };

        public static readonly GH_InstanceDescription DimensionPoints = new GH_InstanceDescription
        {
            Name = "Points",
            NickName = "P",
            Description = "Dimension Points"
        };

        public static readonly GH_InstanceDescription DimensionLocation = new GH_InstanceDescription
        {
            Name = "Location",
            NickName = "L",
            Description = "Dimension Line Location"
        };

        public static readonly GH_InstanceDescription DimensionLinePlacingType = new GH_InstanceDescription
        {
            Name = "Placing Type",
            NickName = "PT",
            Description = $"Placing type of dimension line:\n{EnumHelpers.EnumToString<DimensionSetBaseAttributes.Placings>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription ShortDimensionType = new GH_InstanceDescription
        {
            Name = "Short Dimension Type",
            NickName = "ST",
            Description = $"Position of the short dimension value:\n{EnumHelpers.EnumToString<DimensionSetBaseAttributes.ShortDimensionTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription ExtensionLineType = new GH_InstanceDescription
        {
            Name = "Extension Line Type",
            NickName = "ET",
            Description = $"The extension line type:\n{EnumHelpers.EnumToString<DimensionSetBaseAttributes.ExtensionLineTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription ExcludePartsAccordingToFilter = new GH_InstanceDescription
        {
            Name = "Exclude Filter",
            NickName = "EF",
            Description = "Filter name used for excluding parts in dimension line tags"
        };

        public static readonly GH_InstanceDescription ViewPlane = new GH_InstanceDescription
        {
            Name = "Plane",
            NickName = "P",
            Description = "View plane"
        };

        public static readonly GH_InstanceDescription ViewRestrictionBox = new GH_InstanceDescription
        {
            Name = "Restriction Box",
            NickName = "RB",
            Description = "View restriction box"
        };

        public static readonly GH_InstanceDescription RegenerateObjects = new GH_InstanceDescription
        {
            Name = "Recompute Component",
            NickName = "Recompute",
            Description = "Update the objects in Tekla Structures, regenerating any missing ones"
        };

        public static readonly GH_InstanceDescription BakeToTekla = new GH_InstanceDescription
        {
            Name = "Bake To Tekla",
            NickName = "Bake",
            Description = "Create an independent copy of the model objects that were generated by this component in Tekla Structures"
        };

        public static readonly GH_InstanceDescription DeleteTeklaObjects = new GH_InstanceDescription
        {
            Name = "Delete Objects In Tekla",
            NickName = "Delete",
            Description = "Delete the model objects that were generated by this component in Tekla Structures"
        };

        public static readonly GH_InstanceDescription BakeAllToTekla = new GH_InstanceDescription
        {
            Name = "Bake All Objects To Tekla",
            NickName = "Bake",
            Description = "Create an independent copy of all the generated model objects in Tekla Structures"
        };

        public static readonly GH_InstanceDescription DeleteAllTeklaObjects = new GH_InstanceDescription
        {
            Name = "Delete All Objects In Tekla",
            NickName = "Delete",
            Description = "Delete all objects that are currently being generated in Tekla Structures"
        };

        public static readonly GH_InstanceDescription SelectAllTeklaObjects = new GH_InstanceDescription
        {
            Name = "Select All Objects In Tekla",
            NickName = "Select",
            Description = "Select all objects that are currently being generated in Tekla Structures"
        };

        public static readonly GH_InstanceDescription Points = new GH_InstanceDescription
        {
            Name = "Points",
            NickName = "P",
            Description = "Points"
        };

        public static readonly GH_InstanceDescription HorizontalExtremes = new GH_InstanceDescription
        {
            Name = "Horizontal Extremes",
            NickName = "H",
            Description = "Two extreme points along X"
        };

        public static readonly GH_InstanceDescription VerticalExtremes = new GH_InstanceDescription
        {
            Name = "Vertical Extremes",
            NickName = "V",
            Description = "Two extreme points along Y"
        };

        public static readonly GH_InstanceDescription ModelObject = new GH_InstanceDescription
        {
            Name = "Model Object",
            NickName = "MO",
            Description = "Tekla model object"
        };

        public static readonly GH_InstanceDescription PartReferenceLine = new GH_InstanceDescription
        {
            Name = "Ref Line",
            NickName = "Ref",
            Description = "Part reference line"
        };

        public static readonly GH_InstanceDescription PartCenterLine = new GH_InstanceDescription
        {
            Name = "Center Line",
            NickName = "Cen",
            Description = "Part center line"
        };

        public static readonly GH_InstanceDescription StartPoint = new GH_InstanceDescription
        {
            Name = "Start Point",
            NickName = "S",
            Description = "Part start point"
        };

        public static readonly GH_InstanceDescription EndPoint = new GH_InstanceDescription
        {
            Name = "End Point",
            NickName = "E",
            Description = "Part end point"
        };

        public static readonly GH_InstanceDescription AngleDimensionOriginPoint = new GH_InstanceDescription
        {
            Name = "Origin",
            NickName = "O",
            Description = "Angle dimension origin"
        };

        public static readonly GH_InstanceDescription AngleDimensionPoint1 = new GH_InstanceDescription
        {
            Name = "Point 1",
            NickName = "P1",
            Description = "First point to be used"
        };

        public static readonly GH_InstanceDescription AngleDimensionPoint2 = new GH_InstanceDescription
        {
            Name = "Point 2",
            NickName = "P2",
            Description = "Second point to be used"
        };

        public static readonly GH_InstanceDescription AngleDimensionDistance = new GH_InstanceDescription
        {
            Name = "Distance",
            NickName = "D",
            Description = "Distance of the angle sign from the origin to the first point."
        };

        public static readonly GH_InstanceDescription AngleDimensionAttributes = new GH_InstanceDescription
        {
            Name = "Attributes",
            NickName = "A",
            Description = "Angle Dimension Attributes"
        };

        public static readonly GH_InstanceDescription ObjectMatch = new GH_InstanceDescription
        {
            Name = "Match",
            NickName = "M",
            Description = "Result of matching model object to filter"
        };

        public static readonly GH_InstanceDescription ObjectFilter = new GH_InstanceDescription
        {
            Name = "Filter",
            NickName = "F",
            Description = "Tekla Object Selection Filter"
        };

        public static readonly GH_InstanceDescription DimensionLineAlwaysMode = new GH_InstanceDescription
        {
            Name = "Insert always",
            NickName = "Always",
            Description = "Dimension line will be created despite of projection result"
        };

        public static readonly GH_InstanceDescription DimensionLineMoreThan2PointsMode = new GH_InstanceDescription
        {
            Name = "More than 2 projected points",
            NickName = "ProjectionLimit",
            Description = "Dimension line will be created only when more than 2 projected points"
        };

        public static readonly GH_InstanceDescription PropertyName = new GH_InstanceDescription
        {
            Name = "Property",
            NickName = "P",
            Description = "Property name used for grouping (for UDA or Report mode)"
        };

        public static readonly GH_InstanceDescription GroupingIndices = new GH_InstanceDescription
        {
            Name = "Indices",
            NickName = "I",
            Description = "Grouping Indices"
        };

        public static readonly GH_InstanceDescription GroupingKeys = new GH_InstanceDescription
        {
            Name = "Keys",
            NickName = "K",
            Description = "Grouping Keys"
        };

        public static readonly GH_InstanceDescription GroupByPosition = new GH_InstanceDescription
        {
            Name = "Group by Assembly Position",
            NickName = "Pos",
            Description = "Group by Assembly Position"
        };

        public static readonly GH_InstanceDescription GroupByName = new GH_InstanceDescription
        {
            Name = "Group by Name",
            NickName = "N",
            Description = "Group by Name"
        };

        public static readonly GH_InstanceDescription GroupByClass = new GH_InstanceDescription
        {
            Name = "Group by Class",
            NickName = "C",
            Description = "Group by Class"
        };

        public static readonly GH_InstanceDescription GroupByUDA = new GH_InstanceDescription
        {
            Name = "Group by UDA",
            NickName = "U",
            Description = "Group by UDA"
        };

        public static readonly GH_InstanceDescription GroupByReport = new GH_InstanceDescription
        {
            Name = "Group by Report",
            NickName = "R",
            Description = "Group by Report"
        };
        public static readonly GH_InstanceDescription LeaderLinePresence = new GH_InstanceDescription
        {
            Name = "Leader line",
            NickName = "L",
            Description = "Add text with leader line"
        };

        public static readonly GH_InstanceDescription Text = new GH_InstanceDescription
        {
            Name = "Text",
            NickName = "T",
            Description = "Tekla Text"
        };

        public static readonly GH_InstanceDescription BoundingBox = new GH_InstanceDescription
        {
            Name = "BoundingBox",
            NickName = "BB",
            Description = "Represents the 2D bounding box of the object."
        };
        public static readonly GH_InstanceDescription MarkInsertionPoint = new GH_InstanceDescription
        {
            Name = "Insertion Point",
            NickName = "IP",
            Description = "The insertion point of the mark."
        };
        public static readonly GH_InstanceDescription MarkLeaderLineEndPoint = new GH_InstanceDescription
        {
            Name = "Text Base Point",
            NickName = "BP",
            Description = "The point where the text starts."
        };

        public static readonly GH_InstanceDescription FontFamily = new GH_InstanceDescription
        {
            Name = "Font Family",
            NickName = "F",
            Description = "The font family used (Default Arial)."
        };
        public static readonly GH_InstanceDescription FontSize = new GH_InstanceDescription
        {
            Name = "Font Size",
            NickName = "S",
            Description = "The font size used (Default 2.5)."
        };
        public static readonly GH_InstanceDescription FrameType = new GH_InstanceDescription
        {
            Name = "Frame",
            NickName = "FRM",
            Description = $"The frame of the text: \n{EnumHelpers.EnumToString<FrameTypes>()}\nYou can right-click to set."
        };
        public static readonly GH_InstanceDescription TextAttributes = new GH_InstanceDescription
        {
            Name = "Text Attributes",
            NickName = "TATR",
            Description = "The attributes of the text."
        };
        public static readonly GH_InstanceDescription FontAttributes = new GH_InstanceDescription
        {
            Name = "Font Attributes",
            NickName = "FATR",
            Description = "The attributes of the font used."
        };
        public static readonly GH_InstanceDescription FontWeight = new GH_InstanceDescription
        {
            Name = "Font Weight",
            NickName = "B",
            Description = "The weight of the font, Bold or not (default: non-Bold)"
        };
        public static readonly GH_InstanceDescription FontItalic = new GH_InstanceDescription
        {
            Name = "Font Italic",
            NickName = "I",
            Description = "The italic style of the font, Italic or not (default: non-Italic)"
        };
        public static readonly GH_InstanceDescription BackgroundTransparency = new GH_InstanceDescription
        {
            Name = "Background Mask",
            NickName = "BM",
            Description = "Whether the text has a background mask as Opaque or Transparent."
        };
        public static readonly GH_InstanceDescription Angle = new GH_InstanceDescription
        {
            Name = "Angle",
            NickName = "ANG",
            Description = "Sets a angle value."
        };
        public static readonly GH_InstanceDescription ArrowType = new GH_InstanceDescription
        {
            Name = "Arrow Type",
            NickName = "T",
            Description = $"Sets the type of the arrow: \n{EnumHelpers.EnumToString<ArrowheadTypes>()}\nYou can right-click to set."
        };
        public static readonly GH_InstanceDescription ArrowWidth = new GH_InstanceDescription
        {
            Name = "Width",
            NickName = "W",
            Description = "Sets the width of the arrow."
        };
        public static readonly GH_InstanceDescription ArrowHeight = new GH_InstanceDescription
        {
            Name = "Height",
            NickName = "H",
            Description = "Sets the height of the arrow."
        };
        public static readonly GH_InstanceDescription ArrowAttributes = new GH_InstanceDescription
        {
            Name = "Arrow Attributes",
            NickName = "AA",
            Description = "Sets the atributes of the arrow."
        };
        public static readonly GH_InstanceDescription TextRulerWidth = new GH_InstanceDescription
        {
            Name = "Ruler Width",
            NickName = "RW",
            Description = "Sets the width of the text area."
        };
        public static readonly GH_InstanceDescription ModelView = new GH_InstanceDescription
        {
            Name = "Model View",
            NickName = "V",
            Description = "Tekla Model View"
        };

        public static readonly GH_InstanceDescription ViewCoordinateSystem = new GH_InstanceDescription
        {
            Name = "View CS",
            NickName = "V_CS",
            Description = "View Coordinate System"
        };

        public static readonly GH_InstanceDescription DisplayCoordinateSystem = new GH_InstanceDescription
        {
            Name = "Display CS",
            NickName = "D_CS",
            Description = "Display Coordinate System"
        };

        public static readonly GH_InstanceDescription AllModelViews = new GH_InstanceDescription
        {
            Name = "All model views",
            NickName = "All",
            Description = "All model views from list"
        };

        public static readonly GH_InstanceDescription VisibleModelViews = new GH_InstanceDescription
        {
            Name = "Visible model views",
            NickName = "Vis",
            Description = "Only visible model views from list"
        };

        public static readonly GH_InstanceDescription ViewInsertionPoint = new GH_InstanceDescription
        {
            Name = "Point",
            NickName = "P",
            Description = "View insertion point"
        };

        public static readonly GH_InstanceDescription MeshAttributes = new GH_InstanceDescription
        {
            Name = "Mesh Attributes",
            NickName = "A",
            Description = "Reinforcement mesh attributes"
        };

        public static readonly GH_InstanceDescription MeshVisibilityLongitudinal = new GH_InstanceDescription
        {
            Name = "Visibility Longitudinal",
            NickName = "L",
            Description = $"Sets the visibility of longitudinal rebars: \n{EnumHelpers.EnumToString<ReinforcementBase.ReinforcementVisibilityTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription MeshVisibilityCross = new GH_InstanceDescription
        {
            Name = "Visibility Cross",
            NickName = "C",
            Description = $"Sets the visibility of cross rebars: \n{EnumHelpers.EnumToString<ReinforcementBase.ReinforcementVisibilityTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription MeshReinforcementSymbolIndex = new GH_InstanceDescription
        {
            Name = "Symbol Index",
            NickName = "Idx",
            Description = "Defines the index for the mesh symbol to be used.\nThe index starts from 0 and corresponds to the symbol in the file mesh.sym."
        };

        public static readonly GH_InstanceDescription MeshReinforcementSymbolSize = new GH_InstanceDescription
        {
            Name = "Symbol Size",
            NickName = "Size",
            Description = "Defines the size of the reinforcement mesh symbol."
        };

        public static readonly GH_InstanceDescription RebarAtributes = new GH_InstanceDescription
        {
            Name = "Rebar Attributes",
            NickName = "A",
            Description = "Reinforcement attributes"
        };

        public static readonly GH_InstanceDescription RebarVisibility = new GH_InstanceDescription
        {
            Name = "Visibility",
            NickName = "V",
            Description = $"Sets the visibility of rebars: \n{EnumHelpers.EnumToString<ReinforcementBase.ReinforcementVisibilityTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription StraightEndSymbolTypes = new GH_InstanceDescription
        {
            Name = "Straight Symbol",
            NickName = "SS",
            Description = $"Defines how the straight ends of reinforcing bars should look like: \n{EnumHelpers.EnumToString<ReinforcementBase.StraightEndSymbolTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription HookedEndSymbolTypes = new GH_InstanceDescription
        {
            Name = "Hooked Symbol",
            NickName = "HS",
            Description = $"Defines how the hooked ends of reinforcing bars should look like: \n{EnumHelpers.EnumToString<ReinforcementBase.HookedEndSymbolTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription ReinforcementRepresentationTypes = new GH_InstanceDescription
        {
            Name = "Representation",
            NickName = "R",
            Description = $"The representation of reinforcing bars: \n{EnumHelpers.EnumToString<ReinforcementBase.ReinforcementRepresentationTypes>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription HideLinesHiddenByPart = new GH_InstanceDescription
        {
            Name = "Hidden by Part",
            NickName = "HP",
            Description = "Defines whether lines hidden by parts should be hidden or not."
        };

        public static readonly GH_InstanceDescription HideLinesHiddenByReinforcement = new GH_InstanceDescription
        {
            Name = "Hidden by Rebars",
            NickName = "HR",
            Description = "Defines whether lines hidden by reinforcements should be hidden or not."
        };

        public static readonly GH_InstanceDescription Reinforcement = new GH_InstanceDescription
        {
            Name = "Reinforcement",
            NickName = "R",
            Description = "Drawing reinforcement (single/group/strand/set)"
        };

        public static readonly GH_InstanceDescription Mesh = new GH_InstanceDescription
        {
            Name = "Mesh",
            NickName = "M",
            Description = "Drawing mesh"
        };

        public static readonly GH_InstanceDescription FrameAtributes = new GH_InstanceDescription
        {
            Name = "Frame Attributes",
            NickName = "F",
            Description = "Frame attributes (type and color)"
        };

        public static readonly GH_InstanceDescription SymbolAtributes = new GH_InstanceDescription
        {
            Name = "Symbol Attributes",
            NickName = "S",
            Description = "Symbol attributes"
        };

        public static readonly GH_InstanceDescription SymbolSelection = new GH_InstanceDescription
        {
            Name = "Symbol Selection",
            NickName = "SSel",
            Description = "Symbol info (file name and number)"
        };

        public static readonly GH_InstanceDescription Symbol = new GH_InstanceDescription
        {
            Name = "Symbol",
            NickName = "S",
            Description = "Tekla Symbol"
        };

        public static readonly GH_InstanceDescription InsertionPoint = new GH_InstanceDescription
        {
            Name = "Point",
            NickName = "P",
            Description = "Insertion Point"
        };

        public static readonly GH_InstanceDescription SymbolFile = new GH_InstanceDescription
        {
            Name = "File",
            NickName = "F",
            Description = "Symbol file. E.g. \"xsteel\""
        };

        public static readonly GH_InstanceDescription SymbolIndex = new GH_InstanceDescription
        {
            Name = "Index",
            NickName = "I",
            Description = "Symbol index from 0 to 255."
        };

        public static readonly GH_InstanceDescription PartAttributes = new GH_InstanceDescription
        {
            Name = "Part Attributes",
            NickName = "A",
            Description = "Part attributes"
        };

        public static readonly GH_InstanceDescription PartRepresentation = new GH_InstanceDescription
        {
            Name = "Representation",
            NickName = "R",
            Description = $"Sets the part representation: \n{EnumHelpers.EnumToString<Part.Representation>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription SaveDrawing = new GH_InstanceDescription
        {
            Name = "Save",
            NickName = "S",
            Description = "Should drawing be saved (true by default)"
        };

        public static readonly GH_InstanceDescription DrawingSaveResult = new GH_InstanceDescription
        {
            Name = "Result",
            NickName = "R",
            Description = "True when drawing was closed"
        };

        public static readonly GH_InstanceDescription DrawingDeleteResult = new GH_InstanceDescription
        {
            Name = "Result",
            NickName = "R",
            Description = "True when drawing was deleted"
        };

        public static readonly GH_InstanceDescription DrawingObjectDeleteResult = new GH_InstanceDescription
        {
            Name = "Result",
            NickName = "R",
            Description = "True when drawing object was deleted"
        };

        public static readonly GH_InstanceDescription DistanceBetweenDimensions = new GH_InstanceDescription
        {
            Name = "Distance",
            NickName = "D",
            Description = "Distance between dimensions"
        };

        public static readonly GH_InstanceDescription Bolt = new GH_InstanceDescription
        {
            Name = "Bolt",
            NickName = "B",
            Description = "Tekla drawing Bolt"
        };

        public static readonly GH_InstanceDescription BoltAttributes = new GH_InstanceDescription
        {
            Name = "Bolt Attributes",
            NickName = "A",
            Description = "Bolt attributes"
        };

        public static readonly GH_InstanceDescription BoltRepresentation = new GH_InstanceDescription
        {
            Name = "Representation",
            NickName = "R",
            Description = $"Sets the bolt representation: \n{EnumHelpers.EnumToString<Bolt.Representation>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription SymbolContainsHole = new GH_InstanceDescription
        {
            Name = "Hole visible",
            NickName = "H",
            Description = "True if the bolt symbol contains a hole."
        };

        public static readonly GH_InstanceDescription SymbolContainsAxis = new GH_InstanceDescription
        {
            Name = "Axis visible",
            NickName = "Ax",
            Description = "True if the bolt symbol contains an axis."
        };

        public static readonly GH_InstanceDescription WeldAttributes = new GH_InstanceDescription
        {
            Name = "Weld Attributes",
            NickName = "A",
            Description = "Weld attributes"
        };

        public static readonly GH_InstanceDescription WeldRepresentation = new GH_InstanceDescription
        {
            Name = "Representation",
            NickName = "R",
            Description = $"Sets the weld representation: \n{EnumHelpers.EnumToString<Weld.Representation>()}\nYou can right-click to set."
        };

        public static readonly GH_InstanceDescription DrawHiddenLines = new GH_InstanceDescription
        {
            Name = "Draw Hidden",
            NickName = "DrH",
            Description = "True if hidden lines are drawn."
        };

        public static readonly GH_InstanceDescription DrawOwnHiddenLines = new GH_InstanceDescription
        {
            Name = "Draw Own Hidden",
            NickName = "DrOH",
            Description = "True if own hidden lines are drawn."
        };

        public static readonly GH_InstanceDescription Weld = new GH_InstanceDescription
        {
            Name = "Weld",
            NickName = "W",
            Description = "Tekla drawing Weld"
        };

        public static readonly GH_InstanceDescription SymbolHeight = new GH_InstanceDescription
        {
            Name = "Height",
            NickName = "H",
            Description = "Sets the height of the symbol."
        };

        public static readonly GH_InstanceDescription BoltObject = new GH_InstanceDescription
        {
            Name = "Bolt",
            NickName = "B",
            Description = "Tekla Bolt object"
        };

        public static readonly GH_InstanceDescription BoltSize = new GH_InstanceDescription
        {
            Name = "Size",
            NickName = "S",
            Description = "Bolt size"
        };

        public static readonly GH_InstanceDescription BoltStandard = new GH_InstanceDescription
        {
            Name = "Standard",
            NickName = "Std",
            Description = "Bolt standard"
        };

        public static readonly GH_InstanceDescription BoltType = new GH_InstanceDescription
        {
            Name = "Type",
            NickName = "T",
            Description = "Bolt type"
        };

        public static readonly GH_InstanceDescription BoltPositions = new GH_InstanceDescription
        {
            Name = "Positions",
            NickName = "P",
            Description = "Bolt positions"
        };

        public static readonly GH_InstanceDescription BoltDirection = new GH_InstanceDescription
        {
            Name = "Direction",
            NickName = "D",
            Description = "Bolt direction"
        };

        public static readonly GH_InstanceDescription ReinforcementObject = new GH_InstanceDescription
        {
            Name = "Reinforcement",
            NickName = "R",
            Description = "Tekla Reinforcement object"
        };

        public static readonly GH_InstanceDescription ReinforcementSize = new GH_InstanceDescription
        {
            Name = "Size",
            NickName = "S",
            Description = "Reinforcement size"
        };

        public static readonly GH_InstanceDescription ReinforcementGrade = new GH_InstanceDescription
        {
            Name = "Grade",
            NickName = "G",
            Description = "Reinforcement grade"
        };

        public static readonly GH_InstanceDescription ReinforcementName = new GH_InstanceDescription
        {
            Name = "Name",
            NickName = "N",
            Description = "Reinforcement name"
        };

        public static readonly GH_InstanceDescription ReinforcementType = new GH_InstanceDescription
        {
            Name = "Type",
            NickName = "T",
            Description = "Reinforcement type"
        };

        public static readonly GH_InstanceDescription ReinforcementShape = new GH_InstanceDescription
        {
            Name = "Shape",
            NickName = "Shp",
            Description = "Reinforcement shape"
        };

        public static readonly GH_InstanceDescription ReinforcementGeometry = new GH_InstanceDescription
        {
            Name = "Geometry",
            NickName = "Geo",
            Description = "Reinforcement geometry"
        };

        public static readonly GH_InstanceDescription ReinforcementBendingRadius = new GH_InstanceDescription
        {
            Name = "Bending radius",
            NickName = "R",
            Description = "Reinforcement bending radius"
        };

        public static readonly GH_InstanceDescription SelectionResult = new GH_InstanceDescription
        {
            Name = "Result",
            NickName = "R",
            Description = "True when model object was selected"
        };

        public static readonly GH_InstanceDescription NumberingResult = new GH_InstanceDescription
        {
            Name = "Result",
            NickName = "R",
            Description = "True when numbering was started"
        };

        public static readonly GH_InstanceDescription NumberAllModified = new GH_InstanceDescription
        {
            Name = "All Modified",
            NickName = "All",
            Description = "Number all modified Tekla objects"
        };

        public static readonly GH_InstanceDescription NumberSelected = new GH_InstanceDescription
        {
            Name = "Selected",
            NickName = "Sel",
            Description = "Number only selected, modified Tekla objects"
        };

        public static readonly GH_InstanceDescription AllObjects = new GH_InstanceDescription
        {
            Name = "All objects",
            NickName = "All",
            Description = "Get all objects"
        };

        public static readonly GH_InstanceDescription OnlyVisibleObjects = new GH_InstanceDescription
        {
            Name = "Only visible",
            NickName = "Visible",
            Description = "Get only visible objects (in view restriction box)"
        };

        public static readonly GH_InstanceDescription AssociativeNote = new GH_InstanceDescription
        {
            Name = "Note",
            NickName = "N",
            Description = "Tekla Associative Note"
        };

        public static readonly GH_InstanceDescription DrawingModelObject = new GH_InstanceDescription
        {
            Name = "Drawing Object",
            NickName = "O",
            Description = "Drawing model object (Part/Bolt/Rebar/Weld)"
        };

        public static readonly GH_InstanceDescription MarkAttributes = new GH_InstanceDescription
        {
            Name = "Mark Attributes",
            NickName = "MAttr",
            Description = "The attributes of the Mark"
        };

        public static readonly GH_InstanceDescription DrawingObject = new GH_InstanceDescription
        {
            Name = "Drawing Object",
            NickName = "O",
            Description = "Drawing object"
        };

        public static readonly GH_InstanceDescription MarkAttributesFile = new GH_InstanceDescription
        {
            Name = "Attributes",
            NickName = "Attr",
            Description = "Name of mark attributes file"
        };
    }
}
