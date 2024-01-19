using Grasshopper.Plugin;
using Rhino.Runtime.InProcess;
using System;

namespace Rhino.Inside.Tekla
{
    internal class Program
    {
        private static RhinoCore _rhinoCore;
        private static GH_RhinoScriptInterface _grasshopper;

        static Program()
        {
            RhinoInside.Resolver.Initialize();
        }

        static void Main(string[] args)
        {
            string filePath = @"C:\Users\grzeg\OneDrive\Desktop\Line.gh";
            while (!string.IsNullOrEmpty(Console.ReadLine()))
            {
                InitalizeRhino();
                Solve(filePath);
            }
        }

        private static void InitalizeRhino()
        {
            _rhinoCore ??= new RhinoCore();
        }

        static void Solve(string filePath)
        {
            if (_grasshopper is null)
            {
                _grasshopper = (GH_RhinoScriptInterface)RhinoApp.GetPlugInObject("Grasshopper");
                _grasshopper.RunHeadless();
            }

            _grasshopper.CloseDocument();

            var io = new Grasshopper.Kernel.GH_DocumentIO();
            if (!io.Open(filePath))
                Console.WriteLine("File loading failed.");
            else
            {
                var doc = io.Document;
                doc.Enabled = true;
                doc.NewSolution(false);
            }
        }
    }
}
