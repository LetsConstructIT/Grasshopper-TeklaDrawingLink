using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class MergeMarksComponent : TeklaComponentBaseNew<MergeMarksCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.senary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.MergeMarks;
        public MergeMarksComponent() : base(ComponentInfos.MergeMarksComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var marksToMerge = _command.GetInputValues();
#if API2020
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Merging marks not available in this Tekla version.");
#else
            var status = Tekla.Structures.Drawing.Operations.Operation.MergeMarks(marksToMerge, out List<MarkBase> mergedMarks);
            var nonMergedMarks = FindNonMergedMarks(marksToMerge, mergedMarks);
            _command.SetOutputValues(DA, status, mergedMarks, nonMergedMarks);
#endif
        }

        private List<MarkBase> FindNonMergedMarks(List<MarkBase> marksToMerge, List<MarkBase> mergedMarks)
        {
            var marksLeft = new Dictionary<int, MarkBase>();
            foreach (var item in marksToMerge)
                marksLeft[item.GetId()] = item;

            foreach (var mergedMark in mergedMarks)
            {
                var childObjects = mergedMark.GetObjects(new Type[] { typeof(MarkBase) });
                while (childObjects.MoveNext())
                {
                    var sourceMark = childObjects.Current;
                    if (sourceMark is null)
                        continue;

                    var id = sourceMark.GetId();
                    if (marksLeft.ContainsKey(id))
                        marksLeft.Remove(id);
                }
            }

            return marksLeft.Values.ToList();
        }
    }

    public class MergeMarksCommand : CommandBase
    {
        private readonly InputListParam<MarkBase> _inObjects = new InputListParam<MarkBase>(ParamInfos.Mark);

        private readonly OutputParam<bool> _outResult = new OutputParam<bool>(ParamInfos.MergeResult);
        private readonly OutputListParam<MarkBase> _outMergedMarks = new OutputListParam<MarkBase>(ParamInfos.MergedMarks);
        private readonly OutputListParam<MarkBase> _outNonMergedMarks = new OutputListParam<MarkBase>(ParamInfos.NonMergedMarks);

        internal List<MarkBase> GetInputValues()
        {
            return _inObjects.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, bool result, List<MarkBase> mergedMarks, List<MarkBase> nonMergedMarks)
        {
            _outResult.Value = result;
            _outMergedMarks.Value = mergedMarks;
            _outNonMergedMarks.Value = nonMergedMarks;

            return SetOutput(DA);
        }
    }
}
