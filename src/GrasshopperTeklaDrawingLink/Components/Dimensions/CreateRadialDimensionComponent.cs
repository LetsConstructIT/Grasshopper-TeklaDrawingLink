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

namespace GTDrawingLink.Components.Dimensions
{
    public class CreateRadialDimensionComponent : CreateDatabaseObjectComponentBaseNew<CreateRadialDimensionCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.CreateRadialDimension;

        public CreateRadialDimensionComponent() : base(ComponentInfos.CreateRadialDimensionComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var pts1, var pts2, var pts3, var distances, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, pts1, pts2, pts3, distances, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<RadiusDimension>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertRadiusDimension(views.Get(path),
                                      pts1.Get(i, inputMode),
                                      pts2.Get(i, inputMode),
                                      pts3.Get(i, inputMode),
                                      distances.Get(i, inputMode),
                                      attributes.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private RadiusDimension InsertRadiusDimension(ViewBase view, Point3d p1, Point3d p2, Point3d p3, double distance, string attributesFile)
        {
            var attributes = new RadiusDimensionAttributes(attributesFile);

            var dimension = new RadiusDimension(
                view,
                p1.ToTekla(),
                p2.ToTekla(),
                p3.ToTekla(),
                (double)distance,
                attributes);

            dimension.Insert();

            return dimension;
        }
    }

    public class CreateRadialDimensionCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inArcPoint1 = new InputTreePoint(ParamInfos.ArcPoint1, isOptional: true);
        private readonly InputTreePoint _inArcPoint2 = new InputTreePoint(ParamInfos.ArcPoint2, isOptional: true);
        private readonly InputTreePoint _inArcPoint3 = new InputTreePoint(ParamInfos.ArcPoint3, isOptional: true);
        private readonly InputTreeNumber _inDistances = new InputTreeNumber(ParamInfos.RadialDimensionDistance, isOptional: true);
        private readonly InputTreeString _inAttributes = new InputTreeString(ParamInfos.RadialDimensionAttributes, isOptional: true);

        private readonly OutputTreeParam<RadiusDimension> _outDimensions = new OutputTreeParam<RadiusDimension>(ParamInfos.RadialDimension, 0);

        internal (List<ViewBase> views, TreeData<Point3d> pts1, TreeData<Point3d> pts2, TreeData<Point3d> pts3, TreeData<double> distances, TreeData<string> attributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.Value,
                _inArcPoint1.AsTreeData(),
                _inArcPoint2.AsTreeData(),
                _inArcPoint3.AsTreeData(),
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
