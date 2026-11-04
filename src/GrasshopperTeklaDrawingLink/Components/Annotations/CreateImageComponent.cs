using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateImageComponent : CreateDatabaseObjectComponentBaseNew<CreateImageCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Resources.Image;

        public CreateImageComponent() : base(ComponentInfos.CreateImageComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var paths, var insertionPoints, var imageAttributes, var angles) = _command.GetInputValues(out bool mainInputIsCorrect);
            if (!mainInputIsCorrect)
            {
                HandleMissingInput();
                return null;
            }

            if (!DrawingInteractor.IsInTheActiveDrawing(inputViews.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var views = new ViewCollection<ViewBase>(inputViews);
            var strategy = GetSolverStrategy(false, paths, insertionPoints, imageAttributes, angles);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DatabaseObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var images = InsertImage(views.Get(path),
                                      imageAttributes.Get(i, inputMode),
                                      insertionPoints.Get(i, inputMode),
                                      paths.Get(i, inputMode),
                                      angles.Get(i, inputMode));

                outputObjects.Add(images);
                outputTree.Append(new TeklaDatabaseObjectGoo(images), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private TSD.Image InsertImage(ViewBase view, EmbeddedObjectAttributes attribute, Point3d insertionPoint, string path, double angle)
        {
            var image = new TSD.Image(view, insertionPoint.ToTekla(), path, attribute);

            image.Insert();
            image.ApplyAngle(angle);
            return image;
        }
    }

    public class CreateImageCommand : CommandBase
    {
        private const double _defaultAngle = 0;

        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeString _inPath = new InputTreeString(ParamInfos.Path, isOptional: true);
        private readonly InputTreePoint _inInsertionPoints = new InputTreePoint(ParamInfos.ImageInsertionPoint, isOptional: true);
        private readonly InputTreeParam<EmbeddedObjectAttributes> _inAttributes = new InputTreeParam<EmbeddedObjectAttributes>(ParamInfos.ImageAttributes);

        private readonly InputTreeNumber _inAngles = new InputTreeNumber(ParamInfos.EmbeddedObjectAngle, isOptional: true);

        private readonly OutputTreeParam<TSD.Image> _outText = new OutputTreeParam<TSD.Image>(ParamInfos.Image, 0);

        internal (List<ViewBase> views, TreeData<string> paths, TreeData<Point3d> insertionPoints, TreeData<EmbeddedObjectAttributes> textAttributes, TreeData<double> angles) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                _inPath.AsTreeData(),
                _inInsertionPoints.AsTreeData(),
                _inAttributes.AsTreeData(),
                _inAngles.IsEmpty() ? _inAngles.GetDefault(_defaultAngle) : _inAngles.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure texts)
        {
            _outText.Value = texts;

            return SetOutput(DA);
        }
    }
}
