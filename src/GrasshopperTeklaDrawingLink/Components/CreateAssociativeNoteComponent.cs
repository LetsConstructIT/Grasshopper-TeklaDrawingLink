using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateAssociativeNoteComponent : CreateDatabaseObjectComponentBaseNew<CreateAssociativeNoteCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.AssociativeNote;

        public CreateAssociativeNoteComponent() : base(ComponentInfos.CreateAssociativeNoteComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (modelObjects, attributeFiles, basePoints, leaderLinePoints) = _command.GetInputValues();

            var createdMarks = new List<TSD.Mark>();

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

        private TSD.PlacingBase GetPlacing(List<Point3d> leaderLinePoints, int index)
        {
            if (!leaderLinePoints.Any())
                return TSD.PlacingTypes.PointPlacing();
            else
                return TSD.PlacingTypes.LeaderLinePlacing(leaderLinePoints.ElementAtOrLast(index).ToTekla());
        }

        private TSD.Mark InsertMark(TSD.ModelObject modelObject, string attributeFile, Point3d basePoint, TSD.PlacingBase placing)
        {
            var mark = new TSD.Mark(modelObject)
            {
                InsertionPoint = basePoint.ToTekla(),
                Placing = placing,
                Attributes = new TSD.Mark.MarkAttributes(modelObject, attributeFile)
            };
            mark.Insert();

            return mark;
        }
    }

    public class CreateAssociativeNoteCommand : CommandBase
    {
        private readonly InputListParam<TSD.ModelObject> _inModel = new InputListParam<TSD.ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputListPoint _inInsertionPoints = new InputListPoint(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListPoint _inLeaderLineEndPoints = new InputOptionalListPoint(ParamInfos.MarkLeaderLineEndPoint, new List<Point3d>());
        private readonly InputListParam<string> _inAttributes = new InputListParam<string>(ParamInfos.Attributes);

        private readonly OutputListParam<TSD.Mark> _outMark = new OutputListParam<TSD.Mark>(ParamInfos.AssociativeNote);

        internal (List<TSD.ModelObject> modelObjects, List<string> attributeFiles, List<Point3d> basePoints, List<Point3d> leaderLinePoints) GetInputValues()
        {
            return (
                _inModel.Value,
                _inAttributes.Value,
                _inInsertionPoints.Value,
                _inLeaderLineEndPoints.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSD.Mark> marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
