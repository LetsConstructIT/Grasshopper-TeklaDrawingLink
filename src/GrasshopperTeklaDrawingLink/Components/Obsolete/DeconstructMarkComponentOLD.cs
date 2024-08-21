using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class DeconstructMarkComponentOLD : DeconstructDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructMark;

        public DeconstructMarkComponentOLD() : base(ComponentInfos.DeconstructMarkComponent)
        {
        }
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterDatabaseObjectInputParam(pManager, ParamInfos.Mark);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.DrawingModelObject, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.MarkInsertionPoint, GH_ParamAccess.item);
            AddParameter(pManager, new PlacingBaseParam(ParamInfos.PlacingType, GH_ParamAccess.item));
            AddParameter(pManager, new MarkAttributesParam(ParamInfos.MarkAttributes, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.MarkType, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.AxisAlignedBoundingBox, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.ObjectAlignedBoundingBox, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.LeaderLine, GH_ParamAccess.item);
            AddBooleanParameter(pManager, ParamInfos.IsValid, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<DatabaseObject>(ParamInfos.Mark) is MarkBase mark))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided input could not be converted to MarkBase");
                return;
            }

            mark.Select();
            var objectBoundingBox = mark.GetObjectAlignedBoundingBox();

            var internalMarks = FindInternalMarks(mark);
            var sources = FindMarkSource(internalMarks);

            DA.SetDataList(ParamInfos.DrawingModelObject.Name, sources.Select(s => new TeklaDatabaseObjectGoo(s)));
            DA.SetData(ParamInfos.MarkInsertionPoint.Name, mark.InsertionPoint.ToRhino());
            DA.SetData(ParamInfos.PlacingType.Name, new PlacingBaseGoo(mark.Placing));
            DA.SetData(ParamInfos.MarkAttributes.Name, new MarkAttributesGoo(internalMarks.First().Attributes));
            DA.SetData(ParamInfos.MarkType.Name, GetMarkType(mark));
            DA.SetData(ParamInfos.AxisAlignedBoundingBox.Name, mark.GetAxisAlignedBoundingBox().ToRhino());
            DA.SetData(ParamInfos.ObjectAlignedBoundingBox.Name, objectBoundingBox.ToRhino());
            DA.SetData(ParamInfos.LeaderLine.Name, GuessLeaderLine(mark));
            DA.SetData(ParamInfos.IsValid.Name, objectBoundingBox.Height > 0.1);
        }

        private List<Mark> FindInternalMarks(MarkBase mark)
        {
            var internalMarks = new List<Mark>();
            if (mark is MarkSet)
            {
                var innerMarks = mark.GetObjects(new Type[] { typeof(Mark) });
                while (innerMarks.MoveNext())
                    internalMarks.Add(innerMarks.Current as Mark);
            }
            else if (mark is Mark)
            {
                internalMarks.Add(mark as Mark);
            }

            return internalMarks;
        }

        private List<ModelObject> FindMarkSource(List<Mark> marks)
        {
            var sourceObjects = new List<ModelObject>();
            foreach (var mark in marks)
            {
                var doe = mark.GetRelatedObjects(new Type[] { typeof(ModelObject) });
                while (doe.MoveNext())
                    sourceObjects.Add(doe.Current as ModelObject);
            }

            return sourceObjects;
        }

        private string GetMarkType(MarkBase mark)
        {
            if (mark is MarkSet)
                return "MarkSet";
            else if (mark is Mark)
            {
                if (mark.IsAssociativeNote)
                    return "AssociativeNote";
                else
                    return "Mark";

            }
            else
                return "Unknown";
        }

        private GH_Curve GuessLeaderLine(MarkBase mark)
        {
            var moe = mark.GetObjects(new Type[] { typeof(LeaderLine) });
            while (moe.MoveNext())
                if (moe.Current is LeaderLine leaderLine)
                    return LeaderLineCalculator.GetCurve(leaderLine);

            return new GH_Curve();
        }
    }
}
