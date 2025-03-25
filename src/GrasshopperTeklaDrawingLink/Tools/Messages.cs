namespace GTDrawingLink.Tools
{
    public static class Messages
    {
        public const string Error_ViewFromDifferentDrawing = "The supplied view belongs to a different drawing than the currently opened";

        public const string Remark_EmptyInput = "Main input(s) are empty. Any previously generated objects were deleted from the Tekla model.\r\n    > To preserve generated objects when the input is empty, right-click this component and set 'Remove if input is empty' to 'false'.";

        public const string Warning_NecessaryInputIsMissing = "Necessary data missing. Provide needed arguments.";
    }
}
