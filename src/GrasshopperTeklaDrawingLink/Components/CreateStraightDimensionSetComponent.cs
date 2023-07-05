using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System;
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

        public CreateStraightDimensionSetComponent() : base(ComponentInfos.CreateStraightDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            pManager.AddPointParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.list);
            pManager.AddPointParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.list);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
            SetLastParameterAsOptional(pManager, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.StraightDimensionSet, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return;

            List<Point3d> dimPoints = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.DimensionPoints.Name, dimPoints))
                return;

            List<Point3d> dimLocation = new List<Point3d>();
            if (!DA.GetDataList(ParamInfos.DimensionLocation.Name, dimLocation))
                return;
            if (dimLocation.Count < 2)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Dimension line location requires two points");
                return;
            }

            var attributes = DA.GetGooValue<StraightDimensionSet.StraightDimensionSetAttributes>(ParamInfos.StraightDimensionSetAttributes);

            var pointList = new PointList();
            foreach (var point in dimPoints)
                pointList.Add(point.ToTeklaPoint());

            (Vector vector, double distance) locationProperties = CalculateLocation(dimLocation, dimPoints.First());

            StraightDimensionSet sds = null;
            if (attributes == null)
                sds = _sdsHandler.CreateDimensionSet(view, pointList, locationProperties.vector, locationProperties.distance);
            else
                sds = _sdsHandler.CreateDimensionSet(view, pointList, locationProperties.vector, locationProperties.distance, attributes);

            DrawingInteractor.CommitChanges();

            DA.SetData(ParamInfos.StraightDimensionSet.Name, new TeklaDatabaseObjectGoo(sds));
        }

        private (Vector vector, double distance) CalculateLocation(List<Point3d> dimLineLocation, Point3d dimPoint)
        {
            var line = new Tekla.Structures.Geometry3d.Line(dimLineLocation.First().ToTeklaPoint(), dimLineLocation.Last().ToTeklaPoint());
            var teklaPoint = dimPoint.ToTeklaPoint();
            var projected = Projection.PointToLine(teklaPoint, line);

            var upVector = new Vector(projected - teklaPoint).GetNormal();
            var distance = Distance.PointToPoint(projected, teklaPoint);
            return (upVector, distance);
        }
    }
}
