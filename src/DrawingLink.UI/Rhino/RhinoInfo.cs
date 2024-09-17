using DrawingLink.UI.Exceptions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DrawingLink.UI.Rhino
{
    public class RhinoInfo
    {
        public int Version { get; }
        public int ServiceRelease { get; }
        public string BuildType { get; }
        public string PluginsFolder { get; }
        public string SystemFolder { get; }
        public IEnumerable<string> InstallFolders { get; }

        public RhinoInfo(int version, int serviceRelease, string buildType, string pluginsFolder, string systemFolder, IEnumerable<string> installFolders)
        {
            Version = version;
            ServiceRelease = serviceRelease;
            BuildType = buildType ?? throw new ArgumentNullException(nameof(buildType));
            PluginsFolder = pluginsFolder ?? throw new ArgumentNullException(nameof(pluginsFolder));
            SystemFolder = systemFolder ?? throw new ArgumentNullException(nameof(systemFolder));
            InstallFolders = installFolders ?? throw new ArgumentNullException(nameof(installFolders));
        }

        public static IReadOnlyList<RhinoInfo> GetAllVersions()
        {
            var infos = new List<RhinoInfo>();
            try
            {
                using var registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\McNeel\\Rhinoceros");
                if (registryKey == null)
                    return Array.Empty<RhinoInfo>();

                foreach (var subKey in (from v in registryKey.GetSubKeyNames()
                                        orderby v descending
                                        select v).ToList())
                {
                    using RegistryKey installRegistryKey = registryKey.OpenSubKey(subKey + "\\Install");
                    if (installRegistryKey == null)
                        continue;

                    var rhinoInfo = GetRhinoInfo(installRegistryKey);
                    if (rhinoInfo != null)
                        infos.Add(rhinoInfo);
                }
            }
            catch (Exception ex)
            {
                throw new RhinoSearchException($"Exception while searching for Rhino installers: {ex}");
            }

            return infos;
        }

        private static RhinoInfo? GetRhinoInfo(RegistryKey installRegistryKey)
        {
            if (installRegistryKey.GetValue("BuildType") is not string buildType)
                return null;

            if (installRegistryKey.GetValue("CoreDllPath") is not string coreDllPath)
                return null;

            var systemFolder = Path.GetDirectoryName(coreDllPath);

            var installFolder = new List<string>();
            if (File.Exists(coreDllPath))
                installFolder.Add(systemFolder);

            var pluginFolder = installRegistryKey.GetValue("Default Plug-ins Folder") as string;
            if (pluginFolder != null)
            {
                var ghFolder = Path.Combine(pluginFolder, "Grasshopper");
                if (Directory.Exists(ghFolder))
                    installFolder.Add(ghFolder);
            }

            if (installFolder.Count < 2)
                return null;

            if (installRegistryKey.GetValue("Version") is not string versionString)
                return null;

            var versionNumbers = versionString.Split(new char[] { '.' });
            var version = int.Parse(versionNumbers[0]);
            var serviceRelease = (versionNumbers.Length > 1) ? int.Parse(versionNumbers[1]) : -1;

            return new RhinoInfo(version, serviceRelease, buildType, systemFolder, pluginFolder, installFolder);
        }
    }
}
