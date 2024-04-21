using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
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
            (var views, var points, var fileNames, var attributes) = _command.GetInputValues();
            if (!DrawingInteractor.IsInTheActiveDrawing(views.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

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
        private readonly InputListParam<ViewBase> _inView = new InputListParam<ViewBase>(ParamInfos.View);
        private readonly InputTreePoint _inPoints = new InputTreePoint(ParamInfos.InsertionPoint);
        private readonly InputTreeString _inFileNames = new InputTreeString(ParamInfos.DwgFileName);
        private readonly InputTreeParam<EmbeddedObjectAttributes> _inAttributes = new InputTreeParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);

        private readonly OutputTreeParam<DwgObject> _outDwgs = new OutputTreeParam<DwgObject>(ParamInfos.Dwg, 0);

        internal (ViewCollection<ViewBase> views, TreeData<Rhino.Geometry.Point3d> points, TreeData<string> fileNames, TreeData<EmbeddedObjectAttributes> atrributes) GetInputValues()
        {
            return (new ViewCollection<ViewBase>(_inView.Value),
                    _inPoints.AsTreeData(),
                    _inFileNames.AsTreeData(),
                    _inAttributes.AsTreeData());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure lines)
        {
            _outDwgs.Value = lines;

            return SetOutput(DA);
        }
    }
}
