using Grasshopper.Kernel;

using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using System.Collections.Generic;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text
{
    public class ArrowAttributesComponent : TeklaComponentBase
    {
        public ArrowAttributesComponent()
            : base(ComponentInfos.ArrowAttributesComponent)
        {
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            SetParametersAsOptional(pManager, new List<int> {
            pManager.AddParameter(new EnumParam<ArrowheadTypes>(ParamInfos.ArrowType, GH_ParamAccess.item))
            });
            AddNumberParameter(pManager, ParamInfos.Width, GH_ParamAccess.item, true);
            AddNumberParameter(pManager, ParamInfos.Height, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ArrowAttributesParam(ParamInfos.ArrowAttribute, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var arrowheadAttributes = new ArrowheadAttributes();
            object arrowType = null;
            double height = 0.0;
            double width = 0.0;
            DA.GetData(ParamInfos.ArrowType.Name, ref arrowType);
            DA.GetData(ParamInfos.Height.Name, ref height);
            DA.GetData(ParamInfos.Width.Name, ref width);
           
            var arrowHeadType = EnumHelpers.ObjectToEnumValue<ArrowheadTypes>(arrowType);
            if (arrowHeadType.HasValue)
                arrowheadAttributes.Head = arrowHeadType.Value;
            if (height > 0) arrowheadAttributes.Height = height;
            if (width > 0) arrowheadAttributes.Width = width;

            DA.SetData(ParamInfos.ArrowAttribute.Name, new ArrowAttributesGoo(arrowheadAttributes));
        }
    }
}
