using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class DeconstructMarkComponent : DeconstructDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructMark;

        public DeconstructMarkComponent() : base(ComponentInfos.DeconstructMarkComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterDatabaseObjectInputParam(pManager, ParamInfos.Mark);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.DrawingModelObject, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.MarkInsertionPoint, GH_ParamAccess.item);
            AddParameter(pManager, new PlacingBaseParam(ParamInfos.PlacingType, GH_ParamAccess.item));
            AddParameter(pManager, new MarkAttributesParam(ParamInfos.MarkAttributes, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.MarkType, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.AxisAlignedBoundingBox, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.ObjectAlignedBoundingBox, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.LeaderLine, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<TSD.DatabaseObject>(ParamInfos.Mark) is TSD.MarkBase mark))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided input could not be converted to MarkBase");
                return;
            }

            mark.Select();
            var objectBoundingBox = mark.GetObjectAlignedBoundingBox();

            var internalMarks = FindInternalMarks(mark);
            var sources = FindMarkSource(internalMarks);

            DA.SetDataList(ParamInfos.DrawingModelObject.Name, sources);
            DA.SetData(ParamInfos.MarkInsertionPoint.Name, mark.InsertionPoint.ToRhino());
            DA.SetData(ParamInfos.PlacingType.Name, new PlacingBaseGoo(mark.Placing));
            DA.SetData(ParamInfos.MarkAttributes.Name, new MarkAttributesGoo(internalMarks.First().Attributes));
            DA.SetData(ParamInfos.MarkType.Name, GetMarkType(mark));
            DA.SetData(ParamInfos.AxisAlignedBoundingBox.Name, mark.GetAxisAlignedBoundingBox().ToRhino());
            DA.SetData(ParamInfos.ObjectAlignedBoundingBox.Name, objectBoundingBox.ToRhino());
            DA.SetData(ParamInfos.LeaderLine.Name, GuessLeaderLine(mark));
        }

        private List<TSD.Mark> FindInternalMarks(MarkBase mark)
        {
            var internalMarks = new List<TSD.Mark>();
            if (mark is MarkSet)
            {
                var innerMarks = mark.GetObjects(new Type[] { typeof(TSD.Mark) });
                while (innerMarks.MoveNext())
                    internalMarks.Add(innerMarks.Current as TSD.Mark);
            }
            else if (mark is Mark)
            {
                internalMarks.Add(mark as TSD.Mark);
            }

            return internalMarks;
        }

        private List<TSD.ModelObject> FindMarkSource(List<TSD.Mark> marks)
        {
            var sourceObjects = new List<TSD.ModelObject>();
            foreach (var mark in marks)
            {
                var doe = mark.GetRelatedObjects(new Type[] { typeof(TSD.ModelObject) });
                while (doe.MoveNext())
                    sourceObjects.Add(doe.Current as TSD.ModelObject);
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
