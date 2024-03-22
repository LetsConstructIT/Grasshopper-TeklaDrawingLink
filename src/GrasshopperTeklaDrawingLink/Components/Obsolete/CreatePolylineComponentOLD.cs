using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class CreatePolylineComponentOLD : CreateDatabaseObjectComponentBaseNew<CreatePolylineCommandOLD>
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.Polyline;

        public CreatePolylineComponentOLD() : base(ComponentInfos.CreatePolylineComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (ViewBase View, List<List<TSG.Point>> GroupOfPoints, Polyline.PolylineAttributes Attributes) = _command.GetInputValues();
            var lines = new List<Polyline>();
            if (GroupOfPoints.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The specified point group could not be parsed");
                return lines;
            }

            foreach (var points in GroupOfPoints)
            {
                var pointList = new PointList();
                foreach (var point in points)
                    pointList.Add(point);

                var line = new Polyline(View, pointList, Attributes);
                line.Insert();

                lines.Add(line);
            }

            _command.SetOutputValues(DA, lines);

            DrawingInteractor.CommitChanges();
            return lines;
        }
    }

    public class CreatePolylineCommandOLD : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputListParam<IGH_GeometricGoo> _inGeometricGoo = new InputListParam<IGH_GeometricGoo>(ParamInfos.Curve);
        private readonly InputParam<Polyline.PolylineAttributes> _inAttributes = new InputParam<Polyline.PolylineAttributes>(ParamInfos.PolylineAttributes);

        private readonly OutputListParam<Polyline> _outLines = new OutputListParam<Polyline>(ParamInfos.Polyline);

        internal (ViewBase View, List<List<TSG.Point>> GroupOfPoints, Polyline.PolylineAttributes Attributes) GetInputValues()
        {
            return (_inView.Value,
                    ParseInputTree(_inGeometricGoo.Value),
                    _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Polyline> lines)
        {
            _outLines.Value = lines;

            return SetOutput(DA);
        }

        private List<List<TSG.Point>> ParseInputTree(List<IGH_GeometricGoo> inputObjects)
        {
            var parsed = new List<List<TSG.Point>>();
            foreach (var inputObject in inputObjects)
                parsed.Add(inputObject.GetMergedBoundaryPoints(false).ToList());

            return parsed;
        }
    }
}
