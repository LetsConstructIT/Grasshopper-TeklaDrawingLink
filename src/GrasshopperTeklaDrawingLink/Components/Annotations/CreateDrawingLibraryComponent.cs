using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    public class CreateDrawingLibraryComponent : CreateDatabaseObjectComponentBaseNew<CreateDrawingLibraryCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.DrawingLibrary;

        public CreateDrawingLibraryComponent() : base(ComponentInfos.CreateDrawingLibraryComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var points, var filePaths, var scales) = _command.GetInputValues();
            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var strategy = GetSolverStrategy(false, points, filePaths, scales);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Plugin>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var plugin = InsertPlugin(views.Get(path),
                                          points.Get(i, inputMode),
                                          filePaths.Get(i, inputMode),
                                          scales.Get(i, inputMode));

                outputObjects.Add(plugin);
                outputTree.Append(new TeklaDatabaseObjectGoo(plugin), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private Plugin InsertPlugin(ViewBase View, Point3d Point, string FilePath, double scale)
        {
            var (fileName, directoryName) = ParsePath(FilePath);

            var newPluginInput = new PluginPickerInput();
            newPluginInput.Add(new PickerInputPoint(View, Point.ToTekla()));

            var plugin = new Plugin(View, "DrawingSymbolPlugin");
            plugin.SetPickerInput(newPluginInput);
            plugin.SetAttribute("DetailName", fileName);
            plugin.SetAttribute("Scale", scale);

            string text = directoryName.TrimEnd('\\');
            plugin.SetAttribute("WorkingDirectory1", text.Substring(0, Math.Min(text.Length, 79)));
            plugin.SetAttribute("WorkingDirectory2", text.Length > 79 ? text.Substring(79, Math.Min(text.Length - 79, 79)) : "");
            plugin.SetAttribute("WorkingDirectory3", text.Length > 158 ? text.Substring(158, Math.Min(text.Length - 158, 79)) : "");
            plugin.SetAttribute("WorkingDirectory4", text.Length > 237 ? text.Substring(237, Math.Min(text.Length - 237, 79)) : "");

            plugin.Insert();
            return plugin;
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
        private const double _defaultScale = 1;

        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inPoint = new InputTreePoint(ParamInfos.InsertionPoint);
        private readonly InputTreeString _inFilePath = new InputTreeString(ParamInfos.DetailPath);
        private readonly InputTreeNumber _inScale = new InputTreeNumber(ParamInfos.DetailScale, isOptional: true);

        private readonly OutputTreeParam<DatabaseObject> _outPlugin = new OutputTreeParam<DatabaseObject>(ParamInfos.LibraryPlugin, 0);

        internal (ViewCollection<ViewBase> View, TreeData<Point3d> point, TreeData<string> filePath, TreeData<double> scale) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                    _inPoint.AsTreeData(),
                    _inFilePath.AsTreeData(),
                    _inScale.IsEmpty() ? _inScale.GetDefault(_defaultScale) : _inScale.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure plugin)
        {
            _outPlugin.Value = plugin;

            return SetOutput(DA);
        }
    }
}
