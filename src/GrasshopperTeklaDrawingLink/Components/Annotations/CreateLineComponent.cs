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
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateLineComponent : CreateDatabaseObjectComponentBaseNew<CreateLineCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.Line;

        public CreateLineComponent() : base(ComponentInfos.CreateLineComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var geometries, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, geometries, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Line>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var geometry = geometries.Get(i, inputMode);
                var points = geometry.GetMergedBoundaryPoints(false).ToList();

                var view = views.Get(path);
                var attribute = attributes.Get(i, inputMode);
                var lines = new List<Line>();
                for (int j = 1; j < points.Count; j++)
                {
                    var line = InsertLine(view,
                                          points[j - 1],
                                          points[j],
                                          attribute);
                    lines.Add(line);
                }

                outputObjects.AddRange(lines);
                outputTree.AppendRange(lines.Select(l => new TeklaDatabaseObjectGoo(l)), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Line InsertLine(ViewBase view,
                                       TSG.Point startPoint,
                                       TSG.Point endPoint,
                                       Line.LineAttributes attributes)
        {
            var line = new Line(view, startPoint, endPoint, attributes);
            line.Insert();

            return line;
        }
    }

    public class CreateLineCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreeGeometry _inGeometricGoo = new InputTreeGeometry(ParamInfos.Curve, isOptional: true);
        private readonly InputTreeParam<Line.LineAttributes> _inAttributes = new InputTreeParam<Line.LineAttributes>(ParamInfos.LineAttributes);

        private readonly OutputTreeParam<Line> _outLines = new OutputTreeParam<Line>(ParamInfos.Line, 0);

        internal (List<ViewBase> views, TreeData<IGH_GeometricGoo> geometries, TreeData<Line.LineAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inGeometricGoo.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outLines.Value = lines;

            return SetOutput(DA);
        }
    }
}
