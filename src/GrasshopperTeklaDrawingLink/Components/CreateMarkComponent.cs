using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.DrawingInternal;

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
                var initialMarks = SearchExistingMarks(modelObjects);

                var attribute = attributeFiles.ElementAtOrLast(i);

                DrawingInteractor.Highlight(modelObjects);

                var macroContent = string.Empty;
                var modelObject = modelObjects.First();
                if (modelObject.GetType().InheritsFrom(typeof(ReinforcementBase)))
                    macroContent = Macros.InsertRebarMark(attribute);
                else if (modelObjects.First() is Part)
                    macroContent = Macros.InsertPartMark(attribute);
                else if (modelObjects.First() is Weld)
                    macroContent = Macros.InsertWeldMark(attribute);
                else if (modelObjects.First() is Bolt)
                    macroContent = Macros.InsertBoltMark(attribute);

                if (string.IsNullOrEmpty(macroContent))
                    continue;

                var macroPath = new LightweightMacroBuilder()
                            .SaveMacroAndReturnRelativePath(macroContent);

                Tekla.Structures.Model.Operations.Operation.RunMacro(macroPath);


                var updatedMarks = SearchExistingMarks(modelObjects);

                createdMarks.AddRange(FindNewMarks(updatedMarks, initialMarks));

                // get previously correlated marks
                // check if all modelObjects are the same type (watchout for ReinforcementBase)
                // select all model objects
                // apply macro file for loading
                // insert macro
                // compare new marks with the previous
            }

            _command.SetOutputValues(DA, createdMarks);

            DrawingInteractor.CommitChanges();
            return createdMarks;
        }

        private Dictionary<ModelObject, List<Mark>> SearchExistingMarks(List<ModelObject> modelObjects)
        {
            var result = new Dictionary<ModelObject, List<Mark>>();
            foreach (var modelObject in modelObjects)
            {
                var marks = new List<Mark>();
                var doe = modelObject.GetRelatedObjects(new Type[] { typeof(Mark) });
                while (doe.MoveNext())
                    marks.Add((Mark)doe.Current);

                result[modelObject] = marks;
            }

            return result;
        }

        private IEnumerable<Mark> FindNewMarks(Dictionary<ModelObject, List<Mark>> updatedMarksCollection, Dictionary<ModelObject, List<Mark>> initialMarksCollection)
        {
            var newMarks = new List<Mark>();

            foreach (var updatedMarksPair in updatedMarksCollection)
            {
                var initialIds = initialMarksCollection[updatedMarksPair.Key].Select(m => m.GetIdentifier().ID);

                foreach (Mark newMark in updatedMarksPair.Value)
                {
                    var id = newMark.GetIdentifier().ID;
                    if (!initialIds.Contains(id))
                        newMarks.Add(newMark);
                }
            }

            return newMarks;
        }
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
