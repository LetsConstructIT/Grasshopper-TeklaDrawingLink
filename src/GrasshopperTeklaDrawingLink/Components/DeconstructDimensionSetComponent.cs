using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Linq;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class DeconstructDimensionSetComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public DeconstructDimensionSetComponent() : base(ComponentInfos.DeconstructDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.list);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<DatabaseObject>(ParamInfos.StraightDimensionSet) is StraightDimensionSet sds))
                return;

            sds.Select();

            var dimensionPoints = typeof(StraightDimensionSet).GetProperty("DimensionPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sds) as PointList;
            var rhinoPoints = dimensionPoints.ToArray()
                .Select(p => p.ToRhinoPoint());

            DA.SetDataList(ParamInfos.DimensionPoints.Name, rhinoPoints);
            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(sds.Attributes));
        }
    }
}
