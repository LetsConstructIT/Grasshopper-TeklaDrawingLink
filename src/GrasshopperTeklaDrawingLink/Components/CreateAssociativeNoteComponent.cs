using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateAssociativeNoteComponent : CreateDatabaseObjectComponentBaseNew<CreateAssociativeNoteCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AssociativeNote;

        public CreateAssociativeNoteComponent() : base(ComponentInfos.CreateAssociativeNoteComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (modelObjects, attributeFiles, basePoints, leaderLinePoints) = _command.GetInputValues();

            var createdMarks = new List<Mark>();

            var count = new int[] { modelObjects.Count(), attributeFiles.Count(), basePoints.Count(), leaderLinePoints.Count() }.Max();
            for (int i = 0; i < count; i++)
            {
                var modelObject = modelObjects.ElementAtOrLast(i);
                var insertionPt = basePoints.ElementAtOrLast(i);
                var attribute = attributeFiles.ElementAtOrLast(i);

                var placing = GetPlacing(leaderLinePoints, i);
                var createMarks = InsertMark(modelObject, attribute, insertionPt, placing);

                createdMarks.Add(createMarks);
            }

            _command.SetOutputValues(DA, createdMarks);

            DrawingInteractor.CommitChanges();
            return createdMarks;
        }

        private PlacingBase GetPlacing(List<Point3d> leaderLinePoints, int index)
        {
            if (!leaderLinePoints.Any())
                return PlacingTypes.PointPlacing();
            else
                return PlacingTypes.LeaderLinePlacing(leaderLinePoints.ElementAtOrLast(index).ToTekla());
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
        private readonly InputListParam<ModelObject> _inModel = new InputListParam<ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputListPoint _inInsertionPoints = new InputListPoint(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListPoint _inLeaderLineEndPoints = new InputOptionalListPoint(ParamInfos.MarkLeaderLineEndPoint, new List<Point3d>());
        private readonly InputListParam<Mark.MarkAttributes> _inAttributes = new InputListParam<Mark.MarkAttributes>(ParamInfos.MarkAttributes);

        private readonly OutputListParam<Mark> _outMark = new OutputListParam<Mark>(ParamInfos.AssociativeNote);

        internal (List<ModelObject> modelObjects, List<Mark.MarkAttributes> attributeFiles, List<Point3d> basePoints, List<Point3d> leaderLinePoints) GetInputValues()
        {
            return (
                _inModel.Value,
                _inAttributes.Value,
                _inInsertionPoints.Value,
                _inLeaderLineEndPoints.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Mark> marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
