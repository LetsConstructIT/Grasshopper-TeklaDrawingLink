using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;

using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;

using Rhino.Geometry;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components {
    public class CreateMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateMarkCommand> {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public CreateMarkComponent() : base(ComponentInfos.CreateMarkComponent) { }

        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA) {
            (IEnumerable<TSD.ModelObject> modelObjects,
                IEnumerable<string> attributeFiles,
                IEnumerable<Point3d> basePoints,
                IEnumerable<Point3d> leaderLinePoints)=_command.GetInputValues();

            var createdMarks = new List<TSD.Mark>();

            var count = new int[] { modelObjects.Count(), attributeFiles.Count(), basePoints.Count(), leaderLinePoints.Count() }.Max();
            for(int i = 0; i<count; i++) {
                var modelObject = modelObjects.ElementAtOrLast(i);
                var insertionPt = basePoints.ElementAtOrLast(i);
                var attribute = attributeFiles.ElementAtOrLast(i);

                var createMarks = leaderLinePoints.Any() ?
                    InsertMarkWithLeaderLine(modelObject, attribute, insertionPt, leaderLinePoints.ElementAtOrLast(i)) :
                    InsertMarktWithoutLeaderLine(modelObject, attribute, insertionPt);

                createdMarks.Add(createMarks);
            }

            _command.SetOutputValues(DA, createdMarks);

            DrawingInteractor.CommitChanges();
            return createdMarks;
        }

        private TSD.Mark InsertMarktWithoutLeaderLine(TSD.ModelObject modelObject, string attributeFile, Point3d basePoint) {
            TSD.Mark mark = new TSD.Mark(modelObject);
            mark.InsertionPoint=basePoint.ToTekla();
            mark.Placing=TSD.PlacingTypes.PointPlacing();
            mark.Attributes=new TSD.Mark.MarkAttributes(modelObject, attributeFile);
            mark.Insert();
            return mark;
        }

        private TSD.Mark InsertMarkWithLeaderLine(TSD.ModelObject modelObject, string attributeFile, Point3d basePoint, Point3d leaderLinePoint) {
            TSD.Mark mark = new TSD.Mark(modelObject);
            mark.InsertionPoint=basePoint.ToTekla();
            mark.Placing=TSD.PlacingTypes.LeaderLinePlacing(leaderLinePoint.ToTekla());
            mark.Attributes=new TSD.Mark.MarkAttributes(modelObject, attributeFile);
            mark.Insert();
            return mark;
        }
    }

    public class CreateMarkCommand : CommandBase {
        private readonly InputListParam<TSD.ModelObject> _inModel = new InputListParam<TSD.ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputListParam<string> _inAttributes = new InputListParam<string>(ParamInfos.Attributes);
        private readonly InputListPoint _inInsertionPoints = new InputListPoint(ParamInfos.MarkInsertionPoint);
        private readonly InputOptionalListPoint _inLeaderLineEndPoints = new InputOptionalListPoint(ParamInfos.MarkLeaderLineEndPoint, new List<Point3d>());
        private readonly OutputListParam<TSD.Mark> _outMark = new OutputListParam<TSD.Mark>(ParamInfos.Mark);

        internal (
            IEnumerable<TSD.ModelObject> modelObjects,
            IEnumerable<string> attributeFiles,
            IEnumerable<Point3d> basePoints,
            IEnumerable<Point3d> leaderLinePoints
            ) GetInputValues() {
            return (
                _inModel.Value,
                _inAttributes.Value,
                _inInsertionPoints.Value,
                _inLeaderLineEndPoints.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSD.Mark> marks) {
            _outMark.Value=marks;

            return SetOutput(DA);
        }
    }
}
