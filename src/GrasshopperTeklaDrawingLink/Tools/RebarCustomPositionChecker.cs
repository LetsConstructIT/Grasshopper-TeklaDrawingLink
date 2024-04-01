namespace GTDrawingLink.Tools
{
    public static class RebarCustomPositionChecker
    {
        public static bool IsValid(double customPosition)
        {
            return customPosition >= 0 && customPosition <= 1;
        }
    }
}
