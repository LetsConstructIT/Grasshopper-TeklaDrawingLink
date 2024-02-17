using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Plugins;
using Tekla.Structures.Drawing;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Drawing.UI;
using System.Diagnostics;

namespace TeklaPlugins
{
    [Plugin("GHDrawing2")]
    [PluginUserInterface("TeklaPlugins.GHDrawingForm2")]
    [UpdateMode(UpdateMode.CREATE_ONLY)]
    public class DrawingPlugin : DrawingPluginBase
    {
        public override List<InputDefinition> DefineInput()
        {
            return new List<InputDefinition>();
        }

        public override bool Run(List<InputDefinition> Input)
        {
            Tekla.Structures.Model.Operations.Operation.DisplayPrompt("I'm inside drawing");

            Task.Run( async () =>
            {
                await Task.Delay(10000);
                var test = 5;
            });
            return true;
        }
    }
}
