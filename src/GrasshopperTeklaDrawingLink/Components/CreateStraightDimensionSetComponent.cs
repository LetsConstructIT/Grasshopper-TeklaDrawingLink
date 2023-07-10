using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class CreateStraightDimensionSetComponent : TeklaComponentBase
    {
        private StraightDimensionSetHandler _sdsHandler = new StraightDimensionSetHandler();
        public override GH_Exposure Exposure => GH_Exposure.primary;

        private List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();

        public CreateStraightDimensionSetComponent() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.list);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.item);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            foreach (var item in _insertedObjects)
            {
                item.Delete();
            }
            _insertedObjects.Clear();

            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            List<Point3d> dimPoints = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.DimensionPoints.Name, dimPoints))
                return;

            Rhino.Geometry.Line dimLocation = new Rhino.Geometry.Line();
            if (!DA.GetData(ParamInfos.DimensionLocation.Name, ref dimLocation))
                return;

            var attributes = DA.GetGooValue<StraightDimensionSet.StraightDimensionSetAttributes>(ParamInfos.StraightDimensionSetAttributes);
            if (attributes == null)
                return;

            var pointList = new PointList();
            foreach (var point in dimPoints)
                pointList.Add(point.ToTeklaPoint());

            (Vector vector, double distance) locationProperties = CalculateLocation(dimLocation, dimPoints.First());

            StraightDimensionSet sds = _sdsHandler.CreateDimensionSet(view, pointList, locationProperties.vector, locationProperties.distance, attributes);
            _insertedObjects.Add(sds);
            DrawingInteractor.CommitChanges();

            DA.SetData(ParamInfos.StraightDimensionSet.Name, new TeklaDatabaseObjectGoo(sds));
        }

        private (Vector vector, double distance) CalculateLocation(Rhino.Geometry.Line dimLineLocation, Point3d dimPoint)
        {
            var line = new Tekla.Structures.Geometry3d.Line(dimLineLocation.From.ToTeklaPoint(), dimLineLocation.To.ToTeklaPoint());
            var teklaPoint = dimPoint.ToTeklaPoint();
            teklaPoint.Z = 0;
            var projected = Projection.PointToLine(teklaPoint, line);

            var upVector = new Vector(projected - teklaPoint).GetNormal();
            var distance = Distance.PointToPoint(projected, teklaPoint);
            return (upVector, distance);
        }
    }
}
