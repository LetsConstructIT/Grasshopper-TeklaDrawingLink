using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateTextFileComponent : CreateDatabaseObjectComponentBaseNew<CreateTextFileCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.CreateTextFile;

        public CreateTextFileComponent() : base(ComponentInfos.CreateTextFileComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var paths, var insertionPoints, var textAttributes) = _command.GetInputValues();
            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var strategy = GetSolverStrategy(false, paths, insertionPoints, textAttributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var texts = InsertText(views.Get(path),
                                      textAttributes.Get(i, inputMode),
                                      insertionPoints.Get(i, inputMode),
                                      paths.Get(i, inputMode));

                outputObjects.Add(texts);
                outputTree.Append(new TeklaDatabaseObjectGoo(texts), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private TextFile InsertText(ViewBase view, TextFile.TextFileAttributes attribute, Point3d insertionPoint, string path)
        {
            var textElement = new TextFile(view, insertionPoint.ToTekla(), path, attribute);

            textElement.Insert();
            return textElement;
        }
    }

    public class CreateTextFileCommand : CommandBase
    {
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeString _inPath = new InputTreeString(ParamInfos.Path);
        private readonly InputTreePoint _inInsertionPoints = new InputTreePoint(ParamInfos.MarkInsertionPoint);
        private readonly InputTreeParam<TextFile.TextFileAttributes> _inAttributes = new InputTreeParam<TextFile.TextFileAttributes>(ParamInfos.TextFileAttributes);

        private readonly OutputTreeParam<TSD.TextFile> _outText = new OutputTreeParam<TSD.TextFile>(ParamInfos.TextFile, 0);

        internal (ViewCollection<ViewBase> views, TreeData<string> paths, TreeData<Point3d> insertionPoints, TreeData<TextFile.TextFileAttributes> textAttributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                _inPath.AsTreeData(),
                _inInsertionPoints.AsTreeData(),
                _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure texts)
        {
            _outText.Value = texts;

            return SetOutput(DA);
        }
    }
}
