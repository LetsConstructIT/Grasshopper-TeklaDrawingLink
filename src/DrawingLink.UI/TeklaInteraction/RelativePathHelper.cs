namespace DrawingLink.UI.TeklaInteraction
{
    public class RelativePathHelper
    {
        public static string ExpandRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var trimedPath = path.Trim();
            var modelPath = new Tekla.Structures.Model.Model().GetInfo().ModelPath;
            if (trimedPath.StartsWith(@".\"))
                return $"{modelPath}\\{trimedPath.Substring(2)}";
            else if (trimedPath.StartsWith(@"..\"))
                return $"{modelPath}\\{trimedPath.Substring(3)}";
            else
                return trimedPath;
        }
    }
}
