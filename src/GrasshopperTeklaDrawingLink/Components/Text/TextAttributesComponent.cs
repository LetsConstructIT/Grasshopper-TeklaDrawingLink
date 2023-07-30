using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Tekla.Structures.Drawing;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text {
    public class TextAttributesComponent : TeklaComponentBase {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.LineTypeAttributes;

        public TextAttributesComponent()
            : base(ComponentInfos.TextAttributesComponent) {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.item, true);
            pManager.AddParameter(new FontAttributesParam(ParamInfos.FontAttributes, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            string atrFileName = null;

            DA.GetData(ParamInfos.Attributes.Name, ref atrFileName);

            var textAttributes = new TSD.Text.TextAttributes();

            if(atrFileName!=null) {
                textAttributes=new TSD.Text.TextAttributes(atrFileName);
                DA.SetData(ParamInfos.TextAttributes.Name, new TextAttributesGoo(textAttributes));
                return;
            }

            FontAttributesGoo font = new FontAttributesGoo();
            DA.GetData(ParamInfos.FontAttributes.Name, ref font);
            textAttributes.Font=font.Value;

            DA.SetData(ParamInfos.TextAttributes.Name, new TextAttributesGoo(textAttributes));
        }


    }
}
