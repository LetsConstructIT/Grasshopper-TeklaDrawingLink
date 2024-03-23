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
    public class CreatePolygonComponent : CreateDatabaseObjectComponentBaseNew<CreatePolygonCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Polygon;

        public CreatePolygonComponent() : base(ComponentInfos.CreatePolygonComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var views, var geometries, var attributes) = _command.GetInputValues();

            var strategy = GetSolverStrategy(geometries, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Polygon>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var polyline = InsertPolygon(views.Get(path),
                                              geometries.Get(i, inputMode).GetMergedBoundaryPoints(false),
                                              attributes.Get(i, inputMode));

                outputObjects.Add(polyline);
                outputTree.Append(new TeklaDatabaseObjectGoo(polyline), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Polygon InsertPolygon(ViewBase view,
                                               IEnumerable<TSG.Point> points,
                                               Polygon.PolygonAttributes attributes)
        {
            var pointList = new PointList();
            foreach (var point in points)
                pointList.Add(point);
           
            var line = new Polygon(view, pointList, attributes);
            line.Insert();
            
            return line;
        }
    }

    public class CreatePolygonCommand : CommandBase
    {
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Curve);
        private readonly InputTreeParam<Polygon.PolygonAttributes> _inAttributes = new InputTreeParam<Polygon.PolygonAttributes>(ParamInfos.PolygonAttributes);

        private readonly OutputTreeParam<Polygon> _outPolygons = new OutputTreeParam<Polygon>(ParamInfos.Polygon, 0);

        internal (ViewCollection<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Polygon.PolygonAttributes> atrributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                    _inGeometricGoo.AsTreeData(),
                    _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outPolygons.Value = lines;

            return SetOutput(DA);
        }
    }
}
