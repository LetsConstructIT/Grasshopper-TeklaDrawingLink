using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateAngleDimensionComponent : CreateDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.CreateAngleDimension;

        public CreateAngleDimensionComponent() : base(ComponentInfos.CreateAngleDimensionComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            AddPointParameter(pManager, ParamInfos.AngleDimensionOriginPoint, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.AngleDimensionPoint1, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.AngleDimensionPoint2, GH_ParamAccess.list);
            AddNumberParameter(pManager, ParamInfos.AngleDimensionDistance, GH_ParamAccess.list);
            AddTextParameter(pManager, ParamInfos.AngleDimensionAttributes, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.list));
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return null;

            var dimOriginPts = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.AngleDimensionOriginPoint.Name, dimOriginPts))
                return null;

            var dimPts1 = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.AngleDimensionPoint1.Name, dimPts1))
                return null;

            var dimPts2 = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.AngleDimensionPoint2.Name, dimPts2))
                return null;

            var distances = new List<double>();
            if (!DA.GetDataList(ParamInfos.AngleDimensionDistance.Name, distances))
                return null;

            var attributes = new List<string>();
            DA.GetDataList(ParamInfos.AngleDimensionAttributes.Name, attributes);

            var dimensionNumber = new int[]
            {
                dimOriginPts.Count,
                dimPts1.Count,
                dimPts2.Count,
                distances.Count,
                attributes.Count
            }.Max();

            var insertedDimensions = new AngleDimension[dimensionNumber];
            for (int i = 0; i < dimensionNumber; i++)
            {
                var dimension = InsertAngleDimension(
                    view,
                    dimOriginPts.ElementAtOrLast(i),
                    dimPts1.ElementAtOrLast(i),
                    dimPts2.ElementAtOrLast(i),
                    distances.ElementAtOrLast(i),
                    attributes.HasItems() ? attributes.ElementAtOrLast(i) : "standard");

                insertedDimensions[i] = dimension;
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.StraightDimensionSet.Name, insertedDimensions);

            return insertedDimensions;
        }

        private AngleDimension InsertAngleDimension(View view, Point3d origin, Point3d point1, Point3d point2, double distance, string attributesFile)
        {
            var attributes = new AngleDimensionAttributes(attributesFile);

            var dimension = new AngleDimension(
                view,
                origin.ToTeklaPoint(),
                point1.ToTeklaPoint(),
                point2.ToTeklaPoint(),
                (double)distance,
                attributes);

            dimension.Insert();

            return dimension;
        }
    }
}
