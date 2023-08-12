using Grasshopper.Kernel;

using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using System.Collections.Generic;
using System.Drawing;

using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text {
    public class ArrowAttributeComponent : TeklaComponentBase {


        public ArrowAttributeComponent()
            : base(ComponentInfos.ArrowAttributesComponent) {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.MoveView;


        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            SetParametersAsOptional(pManager, new List<int> {
            pManager.AddParameter(new EnumParam<ArrowheadTypes>(ParamInfos.ArrowType, GH_ParamAccess.item))
            });
            AddNumberParameter(pManager, ParamInfos.Width, GH_ParamAccess.item, true);
            AddNumberParameter(pManager, ParamInfos.Height, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new ArrowAttributesParam(ParamInfos.ArrowAttribute, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            ArrowheadAttributes arrowheadAttributes = new ArrowheadAttributes();
            object arrowType = null;
            DA.GetData(ParamInfos.ArrowType.Name, ref arrowType);
            if(arrowType != null) {
                var arrowHeadType = EnumHelpers.ObjectToEnumValue<ArrowheadTypes>(arrowType);
                if(arrowHeadType.HasValue)
                    arrowheadAttributes.Head = arrowHeadType.Value;
            }

            object height = null;
            DA.GetData(ParamInfos.Height.Name, ref height);
            if(height is Grasshopper.Kernel.Types.GH_Number heightValue) {
                if(heightValue.Value == 0) {
                    arrowheadAttributes.Height = new ArrowheadAttributes().Height;
                }
                else {
                    arrowheadAttributes.Height = heightValue.Value;
                }
            }
            object width = null;
            DA.GetData(ParamInfos.Width.Name, ref width);
            if(width is Grasshopper.Kernel.Types.GH_Number widthValue) {
                if(widthValue.Value == 0) {
                    arrowheadAttributes.Width = new ArrowheadAttributes().Width;
                }
                else {
                    arrowheadAttributes.Width = widthValue.Value;
                }
            }
            DA.SetData(ParamInfos.ArrowAttribute.Name, new ArrowAttributesGoo(arrowheadAttributes));
        }
    }
}
