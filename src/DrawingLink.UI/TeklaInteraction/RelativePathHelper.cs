namespace DrawingLink.UI.TeklaInteraction
{
    public class RelativePathHelper
    {
        public static string ExpandRelativePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var trimedPath = path.Trim();
            var modelPath = GetModelPath();
            if (trimedPath.StartsWith(@".\"))
                return $"{modelPath}\\{trimedPath.Substring(2)}";
            else if (trimedPath.StartsWith(@"..\"))
                return $"{modelPath}\\{trimedPath.Substring(3)}";
            else
                return trimedPath;
        }

        public static string ShortenIfPossible(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var modelPath = GetModelPath();
            if (path.StartsWith(modelPath))
                return path.Replace(modelPath, ".");
            else
                return path;
        }

        private static string GetModelPath()
            => new Tekla.Structures.Model.Model().GetInfo().ModelPath;
    }
}
