﻿using Grasshopper.Kernel;
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

        public static readonly GH_InstanceDescription BooleanTrigger = new GH_InstanceDescription
        {
            Name = "Boolean Trigger",
            NickName = "B",
            Description = "Boolean flag"
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
            Description = "Drawing mark"
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
            Name = "Attributes",
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

        public static readonly GH_InstanceDescription VisibileLineTypeAttributes = new GH_InstanceDescription
        {
            Name = "Visibile lines",
            NickName = "Vis",
            Description = "Part visibile lines attribute"
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
    }
}
