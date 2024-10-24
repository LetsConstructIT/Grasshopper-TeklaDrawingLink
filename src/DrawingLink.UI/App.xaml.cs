using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var rhinoVersions = SetupRhinoFolders();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow(mainWindowViewModel, rhinoVersions);
            new System.Windows.Interop.WindowInteropHelper(mainWindow).Owner = Tekla.Structures.Dialog.MainWindow.Frame.Handle;
            mainWindow.Show();
        }

        private List<Rhino.RhinoInfo> SetupRhinoFolders()
        {
            var versions = Rhino.RhinoInfo.GetAllVersions();
            if (versions.Count == 0)
            {
                MessageBox.Show("No installed Rhino detected. Closing the Grasshopper Application.");
                Environment.Exit(0);
            }

            var neededVersion = versions.Where(v => v.Version == UI.Properties.Settings.Default.RhinoVersion).OrderByDescending(v => v.ServiceRelease).FirstOrDefault();
            if (neededVersion != null)
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
