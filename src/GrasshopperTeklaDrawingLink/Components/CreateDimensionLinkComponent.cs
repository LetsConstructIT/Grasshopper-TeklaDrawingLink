using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDimensionLinkComponent : CreateDatabaseObjectComponentBase
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

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var straightDimensions = DA.GetGooListValue<DatabaseObject>(ParamInfos.StraightDimensionSet).Cast<StraightDimensionSet>().ToList();
            if (straightDimensions == null)
                return null;

            var dimensionLinks = new List<DimensionLink>();
            for (int i = 0; i < straightDimensions.Count; i++)
            {
                for (int j = 0; j < straightDimensions.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (AreParallel(straightDimensions[i], straightDimensions[j]))
                        continue;

                    var dimensionLink = new DimensionLink(straightDimensions[i], straightDimensions[j]);
                    dimensionLinks.Add(dimensionLink);
                }
            }
            dimensionLinks.ForEach(d => d.Insert());

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.StraightDimensionSet.Name, straightDimensions);

            return dimensionLinks;
        }

        private bool AreParallel(StraightDimensionSet straightDimensionSet1, StraightDimensionSet straightDimensionSet2)
        {
            var angle = (180 / Math.PI) * straightDimensionSet1.GetUpDirection().GetAngleBetween(straightDimensionSet2.GetUpDirection());

            var tol = 0.5;
            return angle < tol || angle > 180 - tol;
        }
    }
}
