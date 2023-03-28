using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Model;

namespace GTDrawingLink.Tools
{
    internal class DynamicModelObjectConverter
    {
        internal static Assembly GetAssembly(dynamic input)
        {
            ModelObject modelObject = input.Value as ModelObject;
            if (modelObject == null)
                return null;

            Assembly assembly = null;
            if (modelObject is Assembly)
                assembly = modelObject as Assembly;
            else if (modelObject is Part)
                assembly = (modelObject as Part).GetAssembly();

            if (assembly == null)
                return null;

            return assembly;
        }
    }
}
