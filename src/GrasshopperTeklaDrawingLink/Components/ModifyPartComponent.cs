using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class ModifyPartComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        protected override Bitmap Icon => Resources.ModifyPart;

        public ModifyPartComponent()
            : base(ComponentInfos.ModifyPartComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingPartParam(ParamInfos.TeklaDrawingPart, GH_ParamAccess.list));
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.VisibileLineTypeAttributes, GH_ParamAccess.list));
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.HiddenLineTypeAttributes, GH_ParamAccess.list));
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.ReferenceLineTypeAttributes, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDrawingPartParam(ParamInfos.TeklaDrawingPart, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawingObjects = DA.GetGooListValue<DrawingObject>(ParamInfos.TeklaDrawingPart);
            if (drawingObjects == null || drawingObjects.Count == 0)
                return;

            var visibileLines = DA.GetGooListValue<LineTypeAttributes>(ParamInfos.VisibileLineTypeAttributes);
            var hiddenLines = DA.GetGooListValue<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);
            var referenceLines = DA.GetGooListValue<LineTypeAttributes>(ParamInfos.ReferenceLineTypeAttributes);

            if (!visibileLines.HasItems() && !hiddenLines.HasItems() && !referenceLines.HasItems())
                return;

            for (int i = 0; i < drawingObjects.Count; i++)
            {
                var drawingObject = drawingObjects[i];
                if (!(drawingObject is Part))
                    continue;

                var part = (Part)drawingObject;
                part.Select();

                var visibileLine = GetNthLine(visibileLines, i);
                if (visibileLine != null)
                    part.Attributes.VisibleLines = visibileLine;

                var hiddenLine = GetNthLine(hiddenLines, i);
                if (hiddenLine != null)
                    part.Attributes.HiddenLines = hiddenLine;

                var referenceLine = GetNthLine(referenceLines, i);
                if (referenceLine != null)
                    part.Attributes.ReferenceLine = referenceLine;

                part.Modify();
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.TeklaDrawingPart.Name, drawingObjects.Select(d => new TeklaDrawingObjectGoo(d)));
        }

        private LineTypeAttributes GetNthLine(List<LineTypeAttributes> lineTypeAttributes, int index)
        {
            if (!lineTypeAttributes.HasItems())
                return null;

            if (lineTypeAttributes.Count - 1 < index)
                return lineTypeAttributes.Last();

            return lineTypeAttributes[index];
        }
    }
}
