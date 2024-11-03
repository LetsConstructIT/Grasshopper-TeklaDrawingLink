using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;

namespace GTDrawingLink.Components.Exports
{
    public class OpenFileComponent : TeklaExportComponentBase<OpenFileCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.OpenFile;

        public OpenFileComponent() : base(ComponentInfos.OpenFileComponent)
        {
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var path = _command.GetInputValues();
            if (path.EndsWith(".exe"))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "File execution blocked");
                return;
            }

            if (!System.IO.File.Exists(path))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provided file does not exist");
                return;
            }

            System.Diagnostics.Process.Start(path);

            _command.SetOutputValues(DA, path);
        }
    }

    public class OpenFileCommand : CommandBase
    {
        private readonly InputParam<string> _inPath = new InputParam<string>(ParamInfos.ExportPath);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal string GetInputValues()
        {
            return _inPath.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string path)
        {
            _outPath.Value = path;

            return SetOutput(DA);
        }
    }
}
