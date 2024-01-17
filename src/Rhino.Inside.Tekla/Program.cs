using System;

namespace Rhino.Inside.Tekla
{
    internal class Program
    {
        static Program()
        {
            RhinoInside.Resolver.Initialize();
        }

        static void Main(string[] args)
        {
            using (var core = new Runtime.InProcess.RhinoCore())
            {
                RunHelper();
            }
        }
        static void RunHelper()
        {
            string filePath = @"C:\Users\grzeg\OneDrive\Desktop\Line.gh";

            var pluginObject = RhinoApp.GetPlugInObject("Grasshopper") as Grasshopper.Plugin.GH_RhinoScriptInterface;
            pluginObject.RunHeadless();

            var io = new Grasshopper.Kernel.GH_DocumentIO();
            if (!io.Open(filePath))
                Console.WriteLine("File loading failed.");
            else
            {
                var doc = io.Document;
                doc.Enabled = true;
                doc.NewSolution(false);
            }

            Console.WriteLine("Done... press any key to exit");
            Console.ReadKey();
        }
    }
}
