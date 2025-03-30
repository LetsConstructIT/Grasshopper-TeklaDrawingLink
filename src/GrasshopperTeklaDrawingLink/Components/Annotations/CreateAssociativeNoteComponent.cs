using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class CreateAssociativeNoteComponent : CreateDatabaseObjectComponentBaseNew<CreateAssociativeNoteCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AssociativeNote;

        public CreateAssociativeNoteComponent() : base(ComponentInfos.CreateAssociativeNoteComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (modelObjects, basePoints, placings, attributes) = _command.GetInputValues(out bool mainInputIsCorrect);
            if (!mainInputIsCorrect)
            {
                HandleMissingInput();
                return null;
            }

            if (!DrawingInteractor.IsInTheActiveDrawing(modelObjects.Objects?.First()?.First()))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, Messages.Error_ViewFromDifferentDrawing);
                return null;
            }

            var strategy = GetSolverStrategy(false, modelObjects, attributes, basePoints, placings);
            var inputMode = strategy.Mode;

            var outputTree = new GH_Structure<TeklaDatabaseObjectGoo>();
            var outputObjects = new List<Mark>();

            for (int i = 0; i < strategy.Iterations; i++)
            {
                var path = strategy.GetPath(i);

                var mark = InsertMark(modelObjects.Get(i, inputMode),
                                      attributes.Get(i, inputMode),
                                      basePoints.Get(i, inputMode),
                                      placings.Get(i, inputMode));

                outputObjects.Add(mark);
                outputTree.Append(new TeklaDatabaseObjectGoo(mark), path);
            }

            _command.SetOutputValues(DA, outputTree);

            DrawingInteractor.CommitChanges();
            return outputObjects;
        }

        private Mark InsertMark(ModelObject modelObject, Mark.MarkAttributes attributes, Point3d basePoint, PlacingBase placing)
        {
            var mark = new Mark(modelObject)
            {
                InsertionPoint = basePoint.ToTekla(),
                Placing = placing,
                Attributes = attributes
            };
            mark.Insert();

            return mark;
        }
    }

    public class CreateAssociativeNoteCommand : CommandBase
    {
        private readonly InputOptionalTreeParam<ModelObject> _inModel = new InputOptionalTreeParam<ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputTreePoint _inInsertionPoints = new InputTreePoint(ParamInfos.MarkInsertionPoint, isOptional: true);
        private readonly InputOptionalTreeParam<PlacingBase> _inPlacingTypes = new InputOptionalTreeParam<PlacingBase>(ParamInfos.PlacingType);
        private readonly InputOptionalTreeParam<Mark.MarkAttributes> _inAttributes = new InputOptionalTreeParam<Mark.MarkAttributes>(ParamInfos.MarkAttributes);

        private readonly OutputTreeParam<Mark> _outMark = new OutputTreeParam<Mark>(ParamInfos.AssociativeNote, 0);

        internal (TreeData<ModelObject> modelObjects, TreeData<Point3d> basePoints, TreeData<PlacingBase> placingBases, TreeData<Mark.MarkAttributes> attributes) GetInputValues(out bool mainInputIsCorrect)
        {
            var result = (_inModel.AsTreeData(),
                _inInsertionPoints.AsTreeData(),
                _inPlacingTypes.AsTreeData(),
                _inAttributes.AsTreeData());

            mainInputIsCorrect = result.Item1.HasItems() && result.Item2.HasItems() && result.Item3.HasItems() && result.Item4.HasItems();

            return result;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
