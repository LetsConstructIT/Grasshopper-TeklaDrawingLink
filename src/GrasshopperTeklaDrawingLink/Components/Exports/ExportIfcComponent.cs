using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Exports
{
    public class ExportIfcComponent : TeklaExportComponentBase<ExportIfcCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.ExportIfc;

        public ExportIfcComponent() : base(ComponentInfos.ExportIfcComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (modelObjects, path, settings) = _command.GetInputValues();

            var outputPath = CreateDirectoryIfNeeded(path);

            _command.SetOutputValues(DA, outputPath);
        }

        private static void ExportIFC(string outputFileName, string settings)
        {
            var componentInput = new ComponentInput();
            componentInput.AddOneInputPosition(new Tekla.Structures.Geometry3d.Point(0, 0, 0));

            var comp = new Component(componentInput)
            {
                Name = "ExportIFC",
                Number = BaseComponent.PLUGIN_OBJECT_NUMBER
            };

            comp.LoadAttributesFromFile(settings);
            comp.SetAttribute("OutputFile", outputFileName);
            comp.SetAttribute("CreateAll", 0);  // 0 to export only selected objects

            comp.Insert();
        }
    }

    public class ExportIfcCommand : CommandBase
    {
        private readonly InputOptionalListParam<ModelObject> _inModelObjects = new InputOptionalListParam<ModelObject>(ParamInfos.ModelObject);
        private readonly InputParam<string> _inPath = new InputParam<string>(ParamInfos.ExportPath);
        private readonly InputParam<string> _inSettings = new InputParam<string>(ParamInfos.ExportSettings);

        private readonly OutputParam<string> _outPath = new OutputParam<string>(ParamInfos.ExportResult);

        internal (List<ModelObject> modelObjects, string path, string settings) GetInputValues()
        {
            var modelObjects = _inModelObjects.ValueProvidedByUser ? _inModelObjects.Value : new List<ModelObject>();
            return (
                modelObjects,
                _inPath.Value,
                _inSettings.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string path)
        {
            _outPath.Value = path;

            return SetOutput(DA);
        }
    }
}
