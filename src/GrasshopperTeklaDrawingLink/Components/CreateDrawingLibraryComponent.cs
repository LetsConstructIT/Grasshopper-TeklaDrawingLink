using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDrawingLibraryComponent : CreateDatabaseObjectComponentBaseNew<CreateDrawingLibraryCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.DrawingLibrary;

        public CreateDrawingLibraryComponent() : base(ComponentInfos.CreateDrawingLibraryComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (ViewBase View, Point3d Point, string FilePath, double scale) = _command.GetInputValues();

            var (fileName, directoryName) = ParsePath(FilePath);

            var newPluginInput = new PluginPickerInput();
            newPluginInput.Add(new PickerInputPoint(View, Point.ToTekla()));

            var plugin = new Plugin(View, "DrawingSymbolPlugin");
            plugin.SetPickerInput(newPluginInput);
            plugin.SetAttribute("DetailName", fileName);
            plugin.SetAttribute("Scale", scale);

            string text = directoryName.TrimEnd('\\');
            plugin.SetAttribute("WorkingDirectory1", text.Substring(0, Math.Min(text.Length, 79)));
            plugin.SetAttribute("WorkingDirectory2", (text.Length > 79) ? text.Substring(79, Math.Min(text.Length - 79, 79)) : "");
            plugin.SetAttribute("WorkingDirectory3", (text.Length > 158) ? text.Substring(158, Math.Min(text.Length - 158, 79)) : "");
            plugin.SetAttribute("WorkingDirectory4", (text.Length > 237) ? text.Substring(237, Math.Min(text.Length - 237, 79)) : "");

            plugin.Insert();

            _command.SetOutputValues(DA, plugin);

            DrawingInteractor.CommitChanges();
            return new List<DatabaseObject>() { plugin };
        }

        private (string FileName, string DirectoryName) ParsePath(string filePath)
        {
            if (!filePath.EndsWith(".ddf"))
                filePath += ".ddf";

            var fileName = System.IO.Path.GetFileName(filePath);
            var directoryName = System.IO.Path.GetDirectoryName(filePath);

            return (fileName, directoryName);
        }
    }

    public class CreateDrawingLibraryCommand : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputPoint _inPoint = new InputPoint(ParamInfos.InsertionPoint);
        private readonly InputParam<string> _inFilePath = new InputParam<string>(ParamInfos.DetailPath);
        private readonly InputOptionalStructParam<double> _inScale = new InputOptionalStructParam<double>(ParamInfos.DetailScale, 1);

        private readonly OutputParam<DatabaseObject> _outPlugin = new OutputParam<DatabaseObject>(ParamInfos.LibraryPlugin);

        internal (ViewBase View, Point3d point, string filePath, double scale) GetInputValues()
        {
            return (_inView.Value,
                    _inPoint.Value,
                    _inFilePath.Value,
                    _inScale.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Plugin plugin)
        {
            _outPlugin.Value = plugin;

            return SetOutput(DA);
        }
    }
}
