using DrawingLink.UI.StartupUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DrawingLink.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string[] _rhinoFolders = new string[]
        {
            @"C:\Program Files\Rhino 7\System",
            @"C:\Program Files\Rhino 7\Plug-ins\Grasshopper"
        };

        private CancellationTokenSource? _cts;
        private MainWindow _mainWindow;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var rhinoVersions = SetupRhinoFolders();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var teklaVersion = Tekla.Structures.TeklaStructuresInfo.GetCurrentProgramVersion();
            var startupController = new StartupController(e, teklaVersion);
            var operationMode = startupController.EstablishOperationMode();

            Log(operationMode.ToString());

            var mainWindowViewModel = new MainWindowViewModel();
            _mainWindow = new MainWindow(mainWindowViewModel, rhinoVersions);
            new System.Windows.Interop.WindowInteropHelper(_mainWindow).Owner = Tekla.Structures.Dialog.MainWindow.Frame.Handle;

            var startupOptions = StartupOptions.ParseArguments(e.Args);
            if (operationMode.HasFlag(AppOperationMode.UI))
            {
                if (operationMode.HasFlag(AppOperationMode.Server))
                {
                    _cts = new CancellationTokenSource();
                    StartPipeServer(NamedPipeController.GetPipeName(teklaVersion), _cts.Token);
                }

                _mainWindow.Show();
            }
            else
            {
                if (operationMode.HasFlag(AppOperationMode.Client))
                {
                    if (TrySignalExistingInstance(StartupOptionsDto.FromDomain(startupOptions), teklaVersion))
                    {
                        Shutdown();
                        return;
                    }
                }

                RunScript(startupOptions.SettingsFilePath);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _cts?.Cancel();
        }

        private void RunScript(string settingsFilePath)
        {
            _mainWindow.InitalizeRhino();
            _mainWindow.LoadValues(settingsFilePath);
            _mainWindow.ExecuteScript();
        }

        private static bool TrySignalExistingInstance(StartupOptionsDto startupOptions, string teklaVersion)
        {
            try
            {
                using var client = new NamedPipeClientStream(".",
                    NamedPipeController.GetPipeName(teklaVersion), PipeDirection.Out);

                client.Connect(timeout: 2000);

                using var writer = new StreamWriter(client) { AutoFlush = true };
                writer.Write(JsonConvert.SerializeObject(startupOptions));
                return true;
            }
            catch (TimeoutException)
            {
                return false; // No existing instance, this one should become the server
            }
        }
        public void StartPipeServer(string pipeName, CancellationToken ct)
        {
            Task.Run(async () =>
            {
                while (!ct.IsCancellationRequested)
                {
                    using var server = new NamedPipeServerStream(
                        pipeName,
                        PipeDirection.In,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    Log($"Pipe server initialized: {pipeName}");
                    await server.WaitForConnectionAsync(ct);

                    using var reader = new StreamReader(server);
                    string? message = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(message))
                        System.Windows.Application.Current.Dispatcher.Invoke(() => HandleTrigger(message));
                }
            }, ct);
        }

        private void Log(string message)
        {
            File.AppendAllText(@"C:\temp\PseudoLog.txt", $"{DateTime.Now}: {message}\n");
        }

        private void HandleTrigger(string message)
        {
            Log("Received message: " + message);

            var startupOptions = JsonConvert.DeserializeObject<StartupOptionsDto>(message);
            RunScript(startupOptions.SettingsFilePath);

            Log("Handled message: " + message);
        }

        private List<Rhino.RhinoInfo> SetupRhinoFolders()
        {
            var versionToLaunch = UI.Properties.Settings.Default.RhinoVersion;

            var versions = Rhino.RhinoInfo.GetAllVersions();
            if (versions.Count == 0)
            {
                MessageBox.Show("No installed Rhino detected. Closing the Grasshopper Application.");
                Environment.Exit(0);
            }

            var availableVersions = versions.Select(v => v.Version).Distinct().ToList();
            if (availableVersions.Count == 1)
            {
                versionToLaunch = versions.First().Version;
                UI.Properties.Settings.Default.RhinoVersion = versionToLaunch;
            }

            var neededVersion = versions
                .Where(v => v.Version == versionToLaunch)
                .OrderByDescending(v => v.ServiceRelease)
                .FirstOrDefault() ?? versions.First();

            _rhinoFolders = neededVersion.InstallFolders.ToArray();
            return versions.ToList();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;
            foreach (string rhinoFolder in _rhinoFolders)
            {
                string text = Path.Combine(rhinoFolder, name + ".dll");
                if (File.Exists(text))
                {
                    return Assembly.LoadFrom(text);
                }
            }
            return null;
        }
    }
}
