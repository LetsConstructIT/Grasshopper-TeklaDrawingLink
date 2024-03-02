using System;
using System.IO;
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
            //System.IO.Directory.SetCurrentDirectory(RhinoInside.Resolver.RhinoSystemDirectory);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var mainWindowViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow(mainWindowViewModel);
            mainWindow.Show();
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
