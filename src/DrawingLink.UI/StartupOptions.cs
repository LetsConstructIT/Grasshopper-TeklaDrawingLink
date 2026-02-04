using System;
using Tekla.Structures.Dialog.UIControls;

namespace DrawingLink.UI;
internal class StartupOptions
{
    public string? SettingsFilePath { get; }

    private StartupOptions(string settingsFilePath)
    {
        SettingsFilePath = settingsFilePath;
    }

    public static StartupOptions ParseArguments(string[] startupEventArgs)
    {
        try
        {
            if (startupEventArgs.Length == 2 &&
                startupEventArgs[0] == "-f")
            {
                var filePath = startupEventArgs[1];

                if (System.IO.File.Exists(filePath))
                    return new StartupOptions(filePath);
                else
                {
                    var teklaFilePath = EnvironmentFiles.GetAttributeFile(filePath);
                    if (teklaFilePath != null)
                        return new StartupOptions(teklaFilePath.FullName);
                }
            }
        }
        catch (Exception)
        {
            // Should log failure details
        }

        return new StartupOptions(string.Empty);
    }
}
