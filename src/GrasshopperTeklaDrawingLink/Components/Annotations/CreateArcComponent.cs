using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
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
            (var views, var circles, var attributes) = _command.GetInputValues();
            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

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
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Arc);
        private readonly InputTreeParam<Arc.ArcAttributes> _inAttributes = new InputTreeParam<Arc.ArcAttributes>(ParamInfos.ArcAttributes);

        private readonly OutputTreeParam<Arc> _outArcs = new OutputTreeParam<Arc>(ParamInfos.TeklaArc, 0);

        internal (ViewCollection<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Arc.ArcAttributes> atrributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                    _inGeometricGoo.AsTreeData(),
                    _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure circles)
        {
            _outArcs.Value = circles;

            return SetOutput(DA);
        }
    }
}
