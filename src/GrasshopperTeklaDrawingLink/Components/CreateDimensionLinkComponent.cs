using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDimensionLinkComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateDimensionLink;

        public CreateDimensionLinkComponent() : base(ComponentInfos.CreateDimensionLinkComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.list));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var straightDimensions = DA.GetGooListValue<DatabaseObject>(ParamInfos.StraightDimensionSet).Cast<StraightDimensionSet>().ToList();
            if (straightDimensions == null)
                return;

            for (int i = 0; i < straightDimensions.Count; i++)
            {
                for (int j = 0; j < straightDimensions.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (AreParallel(straightDimensions[i], straightDimensions[j]))
                        continue;

                    new DimensionLink(straightDimensions[i], straightDimensions[j])
                        .Insert();
                }
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.StraightDimensionSet.Name, straightDimensions);
        }

        private bool AreParallel(StraightDimensionSet straightDimensionSet1, StraightDimensionSet straightDimensionSet2)
        {
            var angle = (180 / Math.PI) * straightDimensionSet1.GetUpDirection().GetAngleBetween(straightDimensionSet2.GetUpDirection());

            var tol = 0.5;
            return angle < tol || angle > 180 - tol;
        }
    }
}
