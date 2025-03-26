using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateSymbolComponent : CreateDatabaseObjectComponentBaseNew<CreateSymbolCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.Symbol;

        public CreateSymbolComponent() : base(ComponentInfos.CreateSymbolComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var points, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, points, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Symbol>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var polyline = InsertSymbol(views.Get(path),
                                            points.Get(i, inputMode),
                                            attributes.Get(i, inputMode));

                outputObjects.Add(polyline);
                outputTree.Append(new TeklaDatabaseObjectGoo(polyline), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static Symbol InsertSymbol(ViewBase view,
                                           Rhino.Geometry.Point3d point,
                                           SymbolAttributes attributes)
        {
            var symbol = new Symbol(view, point.ToTekla(), attributes.SymbolInfo, attributes.Attributes);
            symbol.Insert();

            return symbol;
        }
    }

    public class CreateSymbolCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inPoints = new InputTreePoint(ParamInfos.InsertionPoint, isOptional: true);
        private readonly InputTreeParam<SymbolAttributes> _inAttributes = new InputTreeParam<SymbolAttributes>(ParamInfos.SymbolAtributes);

        private readonly OutputTreeParam<Symbol> _outSymbol = new OutputTreeParam<Symbol>(ParamInfos.Symbol, 0);

        internal (List<ViewBase> views, TreeData<Rhino.Geometry.Point3d> points, TreeData<SymbolAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inView.GetValueFromUserOrNull(), _inPoints.AsTreeData(), _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outSymbol.Value = lines;

            return SetOutput(DA);
        }
    }
}
