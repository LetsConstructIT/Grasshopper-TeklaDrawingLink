using System.Text.RegularExpressions;

namespace DrawingLink.UI.StartupUtils;
internal class NamedPipeController
{
    private readonly string _name;

    public NamedPipeController(string teklaVersion)
    {
        _name = GetPipeName(teklaVersion);
    }

    public static string GetPipeName(string teklaVersion)
        => Regex.Replace($"GrasshopperApplication{teklaVersion}_Pipe", @"\s+", ".");    

    public bool DoesPipeExist()
    {
        return System.IO.File.Exists($@"\\.\pipe\{_name}");
    }
}
