using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.VisibileLineTypeAttributes, GH_ParamAccess.item));
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.HiddenLineTypeAttributes, GH_ParamAccess.item));
            AddOptionalParameter(pManager, new LineTypeAttributesParam(ParamInfos.ReferenceLineTypeAttributes, GH_ParamAccess.item));
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

            var visibileLines = DA.GetGooValue<LineTypeAttributes>(ParamInfos.VisibileLineTypeAttributes);
            var hiddenLines = DA.GetGooValue<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);
            var referenceLines = DA.GetGooValue<LineTypeAttributes>(ParamInfos.ReferenceLineTypeAttributes);

            if (visibileLines == null && hiddenLines == null && referenceLines == null)
                return;

            foreach (var item in drawingObjects)
            {
                if (!(item is Part))
                    continue;

                var part = (Part)item;
                part.Select();

                if (visibileLines != null)
                    part.Attributes.VisibleLines = visibileLines;

                if (hiddenLines != null)
                    part.Attributes.HiddenLines = hiddenLines;

                if (referenceLines != null)
                    part.Attributes.ReferenceLine = referenceLines;

                part.Modify();
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.TeklaDrawingPart.Name, drawingObjects.Select(d => new TeklaDrawingObjectGoo(d)));
        }
    }
}
