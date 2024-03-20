using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components
{
    public class SplitGeometryComponent : TeklaComponentBaseNew<SplitGeometryCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SplitGeometry;
        public SplitGeometryComponent() : base(ComponentInfos.SplitGeometryComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<IGH_GeometricGoo> geometries, Point3d splitPoint, Vector3d vector) = _command.GetInputValues();

            var transform = GetTransformation(vector);

            var projectedSplit = new Point3d(splitPoint);
            projectedSplit.Transform(transform);

            var perpPlane = new Plane(projectedSplit, vector);

            var projected = new List<InitialGeometryWithProjectedPoint>();
            for (int i = 0; i < geometries.Count; i++)
            {
                var transformed = geometries[i].DuplicateGeometry().Transform(transform);
                var point = transformed.Boundingbox.Center;

                var intitialGeometry = new InitialGeometryWithProjectedPoint(geometries[i], point);
                intitialGeometry.CalculateDistance(perpPlane);
                projected.Add(intitialGeometry);
            }

            var ordered = projected.OrderBy(p => p.DistanceToPerpendicular).ToList();

            var geometriesA = ordered.Where(o => o.DistanceToPerpendicular < 0).ToList();
            var geometriesB = ordered.Except(geometriesA).ToList();
            geometriesB.Reverse();

            var orderedGeometriesA = geometriesA.Select(o => o.Geometry).ToList();
            var orderedGeometriesB = geometriesB.Select(o => o.Geometry).ToList();

            _command.SetOutputValues(DA,
                                     orderedGeometriesA,
                                     orderedGeometriesB);
        }

        private Transform GetTransformation(Vector3d vector)
        {
            return new Projection(GetPlane(vector)).ToMatrix();
        }

        private Plane GetPlane(Vector3d vector)
        {
            if (vector.IsParallelTo(Vector3d.ZAxis) == 1)
                return new Plane(Point3d.Origin, vector);
            else
                return new Plane(Point3d.Origin, vector, Vector3d.ZAxis);
        }

        private class InitialGeometryWithProjectedPoint
        {
            public IGH_GeometricGoo Geometry { get; }
            public Point3d Projection { get; }
            public double DistanceToPerpendicular { get; private set; }

            public InitialGeometryWithProjectedPoint(IGH_GeometricGoo geometry, Point3d projection)
            {
                Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
                Projection = projection;
            }

            public void CalculateDistance(Plane plane)
            {
                DistanceToPerpendicular = plane.DistanceTo(Projection);
            }
        }
    }

    public class SplitGeometryCommand : CommandBase
    {
        private readonly InputListParam<IGH_GeometricGoo> _inGeometries = new InputListParam<IGH_GeometricGoo>(ParamInfos.Geometry);
        private readonly InputPoint _inPoint = new InputPoint(ParamInfos.SplitPoint);
        private readonly InputStructParam<Vector3d> _inDirection = new InputStructParam<Vector3d>(ParamInfos.DirectionVector);

        private readonly OutputListParam<IGH_GeometricGoo> _outGeometriesA = new OutputListParam<IGH_GeometricGoo>(ParamInfos.GeometryA);
        private readonly OutputListParam<IGH_GeometricGoo> _outGeometriesB = new OutputListParam<IGH_GeometricGoo>(ParamInfos.GeometryB);

        internal (List<IGH_GeometricGoo> InputGeometries, Point3d splitPoint, Vector3d Direction) GetInputValues()
        {
            return (_inGeometries.Value,
                    _inPoint.Value,
                    _inDirection.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<IGH_GeometricGoo> geometriesA, List<IGH_GeometricGoo> geometriesB)
        {
            _outGeometriesA.Value = geometriesA;
            _outGeometriesB.Value = geometriesB;

            return SetOutput(DA);
        }
    }
}
