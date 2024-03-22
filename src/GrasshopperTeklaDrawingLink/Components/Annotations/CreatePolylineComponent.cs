using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Annotations
{
    public class CreatePolylineComponent : CreateDatabaseObjectComponentBaseNew<CreatePolylineCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Polyline;

        public CreatePolylineComponent() : base(ComponentInfos.CreatePolylineComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var geometries, var attributes) = _command.GetInputValues();

            var strategy = GetSolverStrategy(geometries, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Polyline>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var polyline = InsertPolyline(views.Get(path),
                                              geometries.Get(i, inputMode).GetMergedBoundaryPoints(false),
                                              attributes.Get(i, inputMode));

                outputObjects.Add(polyline);
                outputTree.Append(new TeklaDatabaseObjectGoo(polyline), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Polyline InsertPolyline(ViewBase view,
                                               IEnumerable<TSG.Point> points,
                                               Polyline.PolylineAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
                pointList.Add(point);

            var line = new Polyline(view, pointList, attributes);
            line.Insert();

            return line;
        }
    }

    public class CreatePolylineCommand : CommandBase
    {
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Curve);
        private readonly InputTreeParam<Polyline.PolylineAttributes> _inAttributes = new InputTreeParam<Polyline.PolylineAttributes>(ParamInfos.PolylineAttributes);

        private readonly OutputTreeParam<Polyline> _outLines = new OutputTreeParam<Polyline>(ParamInfos.Polyline, 0);

        internal (ViewCollection<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Polyline.PolylineAttributes> atrributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                    _inGeometricGoo.AsTreeData(),
                    _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outLines.Value = lines;

            return SetOutput(DA);
        }
    }
}
