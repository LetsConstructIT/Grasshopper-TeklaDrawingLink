using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateCircleComponent : CreateDatabaseObjectComponentBaseNew<CreateCircleCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Circle;

        public CreateCircleComponent() : base(ComponentInfos.CreateCircleComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var circles, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, circles, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Circle>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var geometry = circles.Get(i, inputMode);
                if (!(geometry is GH_Circle))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "One of the provided objects is not type of Circle");
                    continue;
                }

                var rhinoCircle = (geometry as GH_Circle).Value;

                var view = views.Get(path);
                var attribute = attributes.Get(i, inputMode);

                var circle = InsertCircle(view,
                                          rhinoCircle,
                                          attribute);

                outputObjects.Add(circle);
                outputTree.Append(new TeklaDatabaseObjectGoo(circle), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Circle InsertCircle(ViewBase view,
                                       Rhino.Geometry.Circle rhinoCircle,
                                       Circle.CircleAttributes attributes)
        {
            var circle = new Circle(view, rhinoCircle.Center.ToTekla(), rhinoCircle.Radius.ToTekla(), attributes);
            circle.Insert();

            return circle;
        }
    }

    public class CreateCircleCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Circle, isOptional: true);
        private readonly InputTreeParam<Circle.CircleAttributes> _inAttributes = new InputTreeParam<Circle.CircleAttributes>(ParamInfos.CircleAttributes);

        private readonly OutputTreeParam<Circle> _outCircles = new OutputTreeParam<Circle>(ParamInfos.TeklaCircle, 0);

        internal (List<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Circle.CircleAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inGeometricGoo.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure circles)
        {
            _outCircles.Value = circles;

            return SetOutput(DA);
        }
    }
}
