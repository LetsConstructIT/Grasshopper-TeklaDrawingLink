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
    public class CreateEmbeddedObjectComponent : CreateDatabaseObjectComponentBaseNew<CreateEmbeddedCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.Dwgdxf;

        public CreateEmbeddedObjectComponent() : base(ComponentInfos.CreateEmbeddedObjectComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (var inputViews, var points, var fileNames, var attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
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
            var strategy = GetSolverStrategy(false, points, fileNames, attributes);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<DwgObject>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var dwgObject = InsertDwgObject(views.Get(path),
                                                points.Get(i, inputMode),
                                                fileNames.Get(i, inputMode),
                                                attributes.Get(i, inputMode));

                outputObjects.Add(dwgObject);
                outputTree.Append(new TeklaDatabaseObjectGoo(dwgObject), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private static DwgObject InsertDwgObject(ViewBase view,
                                                 Rhino.Geometry.Point3d point,
                                                 string fileName,
                                                 EmbeddedObjectAttributes attributes)
        {
            var dwgObject = new DwgObject(view, point.ToTekla(), fileName, attributes);
            dwgObject.Insert();

            return dwgObject;
        }
    }

    public class CreateEmbeddedCommand : CommandBase
    {
        private readonly InputOptionalListParam<ViewBase> _inView = new InputOptionalListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inPoints = new InputTreePoint(ParamInfos.InsertionPoint, isOptional: true);
        private readonly InputTreeString _inFileNames = new InputTreeString(ParamInfos.DwgFileName);
        private readonly InputOptionalTreeParam<EmbeddedObjectAttributes> _inAttributes = new InputOptionalTreeParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);

        private readonly OutputTreeParam<DwgObject> _outDwgs = new OutputTreeParam<DwgObject>(ParamInfos.Dwg, 0);

        internal (List<ViewBase> views, TreeData<Rhino.Geometry.Point3d> points, TreeData<string> fileNames, TreeData<EmbeddedObjectAttributes> atrributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result= (_inView.GetValueFromUserOrNull(),
                    _inPoints.AsTreeData(),
                    _inFileNames.AsTreeData(),
                    _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outDwgs.Value = lines;

            return SetOutput(DA);
        }
    }
}
