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

namespace GTDrawingLink.Components.Dimensions
{
    public class CreateAngleDimensionComponent : CreateDatabaseObjectComponentBaseNew<CreateAngleDimensionCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.CreateAngleDimension;

        public CreateAngleDimensionComponent() : base(ComponentInfos.CreateAngleDimensionComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var originPts, var pts1, var pts2, var distances, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, originPts, pts1, pts2, distances, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<AngleDimension>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertAngleDimension(views.Get(path),
                                      originPts.Get(i, inputMode),
                                      pts1.Get(i, inputMode),
                                      pts2.Get(i, inputMode),
                                      distances.Get(i, inputMode),
                                      attributes.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private AngleDimension InsertAngleDimension(ViewBase view, Point3d origin, Point3d point1, Point3d point2, double distance, string attributesFile)
        {
            var attributes = new AngleDimensionAttributes(attributesFile);

            var dimension = new AngleDimension(
                view,
                origin.ToTekla(),
                point1.ToTekla(),
                point2.ToTekla(),
                (double)distance,
                attributes);

            dimension.Insert();

            return dimension;
        }
    }

    public class CreateAngleDimensionCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inOriginPoints = new InputTreePoint(ParamInfos.AngleDimensionOriginPoint);
        private readonly InputTreePoint _inDimPoints1 = new InputTreePoint(ParamInfos.AngleDimensionPoint1);
        private readonly InputTreePoint _inDimPoints2 = new InputTreePoint(ParamInfos.AngleDimensionPoint2);
        private readonly InputTreeNumber _inDistances = new InputTreeNumber(ParamInfos.AngleDimensionDistance);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.AngleDimensionAttributes, isOptional: true);

        private readonly OutputTreeParam<AngleDimension> _outDimensions = new OutputTreeParam<AngleDimension>(ParamInfos.AngleDimension, 0);

        internal (List<ViewBase> views, TreeData<Point3d> originPts, TreeData<Point3d> pts1, TreeData<Point3d> pts2, TreeData<double> distances, TreeData<string> attributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(),
                _inOriginPoints.AsTreeData(),
                _inDimPoints1.AsTreeData(),
                _inDimPoints2.AsTreeData(),
                _inDistances.AsTreeData(),
                _inAttributes.IsEmpty() ? _inAttributes.GetDefault("standard") : _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems() && result.Item4.HasItems() && result.Item5.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure dimensions)
        {
            _outDimensions.Value = dimensions;

            return SetOutput(DA);
        }
    }
}
