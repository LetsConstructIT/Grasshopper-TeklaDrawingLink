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
    public class CreateArcComponent : CreateDatabaseObjectComponentBaseNew<CreateArcCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Arc;

        public CreateArcComponent() : base(ComponentInfos.CreateArcComponent) { }

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
            var outputObjects = new List<Arc>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var geometry = circles.Get(i, inputMode);
                if (!(geometry is GH_Arc))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "One of the provided objects is not type of Circle");
                    continue;
                }

                var rhinoArc = (geometry as GH_Arc).Value;

                var view = views.Get(path);
                var attribute = attributes.Get(i, inputMode);

                var circle = InsertArc(view,
                                       rhinoArc,
                                       attribute);

                outputObjects.Add(circle);
                outputTree.Append(new TeklaDatabaseObjectGoo(circle), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Arc InsertArc(ViewBase view,
                                     Rhino.Geometry.Arc rhinoArc,
                                     Arc.ArcAttributes attributes)
        {
            var circle = new Arc(view,
                                 rhinoArc.EndPoint.ToTekla(),
                                 rhinoArc.StartPoint.ToTekla(),
                                 rhinoArc.Center.ToTekla(),
                                 attributes);
            circle.Insert();

            return circle;
        }
    }

    public class CreateArcCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Arc, isOptional: true);
        private readonly InputTreeParam<Arc.ArcAttributes> _inAttributes = new InputTreeParam<Arc.ArcAttributes>(ParamInfos.ArcAttributes);

        private readonly OutputTreeParam<Arc> _outArcs = new OutputTreeParam<Arc>(ParamInfos.TeklaArc, 0);

        internal (List<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Arc.ArcAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inGeometricGoo.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure circles)
        {
            _outArcs.Value = circles;

            return SetOutput(DA);
        }
    }
}
