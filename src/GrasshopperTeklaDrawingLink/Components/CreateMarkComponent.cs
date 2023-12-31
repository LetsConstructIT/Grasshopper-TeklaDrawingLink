using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Mark;

        public CreateMarkComponent() : base(ComponentInfos.CreateMarkComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var (modelObjectsTree, attributeFiles) = _command.GetInputValues();

            var createdMarks = new List<Mark>();

            var count = new int[] { modelObjectsTree.Count(), attributeFiles.Count() }.Max();
            for (int i = 0; i < count; i++)
            {
                var modelObjects = modelObjectsTree.ElementAtOrLast(i);
                var attribute = attributeFiles.ElementAtOrLast(i);

                //var placing = GetPlacing(leaderLinePoints, i);
                //var createMarks = InsertMark(modelObject, attribute, insertionPt, placing);

                //createdMarks.Add(createMarks);
            }

            _command.SetOutputValues(DA, createdMarks);

            DrawingInteractor.CommitChanges();
            return createdMarks;
        }

        //private Mark InsertMark(ModelObject modelObject, Mark.MarkAttributes attributes, Point3d basePoint, PlacingBase placing)
        //{
        //    var mark = new Mark(modelObject)
        //    {
        //        InsertionPoint = basePoint.ToTekla(),
        //        Placing = placing,
        //        Attributes = attributes
        //    };
        //    mark.Insert();

        //    return mark;
        //}
    }

    public class CreateMarkCommand : CommandBase
    {
        private readonly InputTreeParam<ModelObject> _inModel = new InputTreeParam<ModelObject>(ParamInfos.DrawingModelObject);
        private readonly InputListParam<string> _inAttributes = new InputListParam<string>(ParamInfos.MarkAttributesFile);

        private readonly OutputListParam<Mark> _outMark = new OutputListParam<Mark>(ParamInfos.Mark);

        internal (List<List<ModelObject>> modelObjects, List<string> attributeFiles) GetInputValues()
        {
            return (_inModel.Value,
                    _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Mark> marks)
        {
            _outMark.Value = marks;

            return SetOutput(DA);
        }
    }
}
