using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Annotations
{
    public class DeconstructTextComponent : DeconstructDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructText;

        public DeconstructTextComponent() : base(ComponentInfos.DeconstructTextComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterDatabaseObjectInputParam(pManager, ParamInfos.Text);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.TextContents, GH_ParamAccess.item);
            AddPointParameter(pManager, ParamInfos.MarkInsertionPoint, GH_ParamAccess.item);
            AddParameter(pManager, new PlacingBaseParam(ParamInfos.PlacingType, GH_ParamAccess.item));
            AddParameter(pManager, new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.item));
            AddCurveParameter(pManager, ParamInfos.AxisAlignedBoundingBox, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.ObjectAlignedBoundingBox, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<TSD.DatabaseObject>(ParamInfos.Text) is TSD.Text text))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided input could not be converted to Tekla Text object");
                return;
            }

            text.Select();

            DA.SetData(ParamInfos.TextContents.Name, text.TextString);
            DA.SetData(ParamInfos.MarkInsertionPoint.Name, text.InsertionPoint.ToRhino());
            DA.SetData(ParamInfos.PlacingType.Name, new PlacingBaseGoo(text.Placing));
            DA.SetData(ParamInfos.TextAttributes.Name, new TextAttributesGoo(text.Attributes));
            DA.SetData(ParamInfos.AxisAlignedBoundingBox.Name, text.GetAxisAlignedBoundingBox().ToRhino());
            DA.SetData(ParamInfos.ObjectAlignedBoundingBox.Name, text.GetObjectAlignedBoundingBox().ToRhino());
        }
    }
}
