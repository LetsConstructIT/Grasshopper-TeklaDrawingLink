using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Grasshopper.Kernel.Types;

namespace GTDrawingLink.Components
{
    public class DeconstructDimensionSetComponent : DeconstructDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public DeconstructDimensionSetComponent() : base(ComponentInfos.DeconstructDimensionSetComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterDatabaseObjectInputParam(pManager, ParamInfos.StraightDimensionSet);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(ParamInfos.DimensionPoints.Name, ParamInfos.DimensionPoints.NickName, ParamInfos.DimensionPoints.Description, GH_ParamAccess.item);
            pManager.AddLineParameter(ParamInfos.DimensionLocation.Name, ParamInfos.DimensionLocation.NickName, ParamInfos.DimensionLocation.Description, GH_ParamAccess.item);
            pManager.AddParameter(new StraightDimensionSetAttributesParam(ParamInfos.StraightDimensionSetAttributes, GH_ParamAccess.item));
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!(DA.GetGooValue<TSD.DatabaseObject>(ParamInfos.StraightDimensionSet) is TSD.StraightDimensionSet sds))
                return;

            sds.Select();

            var dimensionPoints = (typeof(TSD.StraightDimensionSet).GetProperty("DimensionPoints", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sds) as TSD.PointList).ToArray();

            DA.SetData(ParamInfos.DimensionPoints.Name, new GH_Curve(new Rhino.Geometry.Polyline(dimensionPoints.Select(p => p.ToRhinoPoint())).ToPolylineCurve()));
            DA.SetData(ParamInfos.DimensionLocation.Name, GetDimensionLocation(sds, dimensionPoints));
            DA.SetData(ParamInfos.StraightDimensionSetAttributes.Name, new StraightDimensionSetAttributesGoo(sds.Attributes));
        }

        private Rhino.Geometry.Line GetDimensionLocation(TSD.StraightDimensionSet sds, Point[] dimPoints)
        {
            var upDirection = (typeof(TSD.StraightDimensionSet).GetProperty("UpDirection", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(sds) as Vector);
            var dimLineDirection = upDirection.Cross(new Vector(0, 0, 1));

            var initialPoint = dimPoints.First() + sds.Distance * upDirection;
            var dimLineCoordSystem = new CoordinateSystem(initialPoint, dimLineDirection, upDirection);

            var toDimLocationCs = MatrixFactory.ToCoordinateSystem(dimLineCoordSystem);

            var dimLine = new Line(initialPoint, dimLineDirection);
            var projectedPoints = dimPoints.Select(p => Projection.PointToLine(p, dimLine)).ToList();
            var localPoints = projectedPoints.Select(p => toDimLocationCs.Transform(p)).ToList();
            var orderedPoints = localPoints.OrderBy(p => p.X);

            var firstPt = projectedPoints[localPoints.IndexOf(orderedPoints.First())];
            var lastPt = projectedPoints[localPoints.IndexOf(orderedPoints.Last())];

            firstPt.Z = 0;
            lastPt.Z = 0;
            return new Rhino.Geometry.Line(firstPt.ToRhinoPoint(), lastPt.ToRhinoPoint());
        }
    }
}
