using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Tekla.Structures;
using TSM = Tekla.Structures.Model;
using Tekla.Structures.Model;

namespace Tekla.Technology.Akit.UserScript
{
    public class Script
    {
        public static void Run(Tekla.Technology.Akit.IScript akit)
        {
            string TSBinaryDir = "";
            TSM.Model CurrentModel = new TSM.Model();
            TeklaStructuresSettings.GetAdvancedOption("XSDATADIR", ref TSBinaryDir);
			
            string ApplicationName = "Grasshopper Application.exe";
            string ApplicationPath = Path.Combine(TSBinaryDir, "Environments\\common\\extensions\\GrasshopperApplication\\" + ApplicationName);

            Process NewProcess = new Process();

            if (File.Exists(ApplicationPath))
            {
                NewProcess.StartInfo.FileName = ApplicationPath;

                try
                {
                    NewProcess.Start();
                    NewProcess.WaitForExit();
                }
                catch
                {
                    MessageBox.Show(ApplicationName + " failed to start.");
                }
            }
            else
            {
                MessageBox.Show(ApplicationName + " not found.");
            }
        }
    }
}