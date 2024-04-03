using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Linq;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Dimensions
{
    public class DeconstructDimensionSetComponent : DeconstructDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.DeconstructDimensionSet;

        public DeconstructDimensionSetComponent() : base(ComponentInfos.DeconstructDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterDatabaseObjectInputParam(pManager, ParamInfos.StraightDimensionSet);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.list);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.item);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<TSD.DatabaseObject>(ParamInfos.StraightDimensionSet) is TSD.StraightDimensionSet sds))
                return;

            sds.Select();

            var dimensionPoints = sds.GetPoints();

            DA.SetDataList(ParamInfos.DimensionPoints.Name, dimensionPoints.Select(p => p.ToRhino()));
            DA.SetData(ParamInfos.DimensionLocation.Name, sds.GetDimensionLocation(dimensionPoints).ToRhino());
            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(sds.Attributes));
        }
    }
}
