using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DrawingLink.UI.Utils;
internal static class LoopReflection
{
    public static bool HasLoopEnded(IGH_Component component)
    {
        var output = component.Params.Output[0].VolatileData
            .AllData(skipNulls: true).ToList();
        if (output.Count == 0)
            return true;

        if (output.First() is GH_Boolean boolean)
        {
            return boolean.Value;
        }

        return false;
    }

    public static object? FindLoopStart(IGH_Component component)
    {
        var type = component.GetType();
        var loopField = type.GetField("_loopStart", BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (loopField == null)
            return null;

        var foundValue = loopField.GetValue(component);
        return foundValue;
    }
}
