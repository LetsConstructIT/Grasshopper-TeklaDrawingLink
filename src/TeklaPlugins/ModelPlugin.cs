using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Plugins;

namespace TeklaPlugins
{
    [Plugin("GHDrawing")]
    [PluginUserInterface("TeklaPlugins.GHDrawingForm")]
    [InputObjectDependency(InputObjectDependency.NOT_DEPENDENT)]
    public class ModelPlugin : PluginBase
    {
        public override List<InputDefinition> DefineInput()
        {
            return new List<InputDefinition>();
        }

        public override bool Run(List<InputDefinition> Input)
        {
            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("I'm inside model");


            return true;
        }
    }
}
