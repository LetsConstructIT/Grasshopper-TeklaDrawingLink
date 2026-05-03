using System.Windows;

namespace DrawingLink.UI.StartupUtils;
internal class StartupController
{
    private readonly StartupOptions _startupOptions;
    private readonly string _teklaVersion;
    private NamedPipeController? _namedPipeController;

    public StartupController(StartupEventArgs e, string teklaVersion)
    {
        _startupOptions = StartupOptions.ParseArguments(e.Args);
        _teklaVersion = teklaVersion;
    }

    public AppOperationMode EstablishOperationMode()
    {
        if (string.IsNullOrWhiteSpace(_startupOptions.SettingsFilePath)) // UI Mode
        {
            if (MutexController.ShouldRunAsServer(_teklaVersion))
                return AppOperationMode.UI | AppOperationMode.Server;
            else
                return AppOperationMode.UI;
        }
        else // Background Mode
        {
            // Check if there is an active server
            _namedPipeController = new NamedPipeController(_teklaVersion);
            if (_namedPipeController.DoesPipeExist())
                return AppOperationMode.Background | AppOperationMode.Client;
            else
                return AppOperationMode.Background;
        }
    }
}
