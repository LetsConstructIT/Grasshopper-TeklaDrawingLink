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
    public class CreateLineComponentOLD : CreateDatabaseObjectComponentBaseNew<CreateLineCommandOLD>
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.Line;

        public CreateLineComponentOLD() : base(ComponentInfos.CreateLineComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (ViewBase View, List<List<TSG.Point>> GroupOfPoints, Line.LineAttributes Attributes) = _command.GetInputValues();
            var lines = new List<Line>();
            if (GroupOfPoints.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The specified point group could not be parsed");
                return lines;
            }

            foreach (var points in GroupOfPoints)
            {
                for (int i = 1; i < points.Count; i++)
                {
                    var line = new Line(View, points[i - 1], points[i], Attributes);
                    line.Insert();

                    lines.Add(line);
                }
            }

            _command.SetOutputValues(DA, lines);

            DrawingInteractor.CommitChanges();
            return lines;
        }
    }

    public class CreateLineCommandOLD : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputListParam<IGH_GeometricGoo> _inGeometricGoo = new InputListParam<IGH_GeometricGoo>(ParamInfos.Curve);
        private readonly InputParam<Line.LineAttributes> _inAttributes = new InputParam<Line.LineAttributes>(ParamInfos.LineAttributes);

        private readonly OutputListParam<Line> _outLines = new OutputListParam<Line>(ParamInfos.Line);

        internal (ViewBase View, List<List<TSG.Point>> GroupOfPoints, Line.LineAttributes Attributes) GetInputValues()
        {
            return (_inView.Value,
                    ParseInputTree(_inGeometricGoo.Value),
                    _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Line> lines)
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
