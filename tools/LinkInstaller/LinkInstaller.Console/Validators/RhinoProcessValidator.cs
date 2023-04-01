using System.Diagnostics;
using System.Linq;

namespace LinkInstaller.Validators
{
    public class RhinoProcessValidator : MessageBoxValidator
    {
        public override bool IsValid()
        {
            var isAnyRhinoRunning = Process
                .GetProcesses()
                .Any(p => p.ProcessName.StartsWith("Rhino"));

            if (isAnyRhinoRunning)
            {
                ShowErrorToUser(new string[]
                {
                    "Rhino is already running.",
                    "In order to updated .gha files you have to close it."
                });

                return false;
            }

            return true;
        }
    }
}
