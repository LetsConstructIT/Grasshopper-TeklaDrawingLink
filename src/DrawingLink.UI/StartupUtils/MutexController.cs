using System;
using System.Threading;

namespace DrawingLink.UI.StartupUtils;

/// <summary>
/// Only the UI server mode should have single instance
/// The idea behind this class is to tell if some other server is already running
/// </summary>
internal class MutexController
{
    private static Mutex? _mutex;

    public static bool ShouldRunAsServer(string teklaVersion)
    {
        var name = EstablishMutexName(teklaVersion);

        var mutex = new Mutex(true, name, out bool isFirstInstance);
        if (isFirstInstance)
        {
            _mutex = mutex;
            return true;
        }
        else
        {
            mutex.Dispose();

            return false;
        }        
    }

    private static string EstablishMutexName(string teklaVersion)
    {
        return $"GrasshopperApplication{teklaVersion}_SingleInstance";
    }
}
