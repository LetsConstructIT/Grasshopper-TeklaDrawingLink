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
    public class SortByVectorComponent : TeklaComponentBaseNew<SortByVectorCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SortByVector;
        public SortByVectorComponent() : base(ComponentInfos.SortByVectorComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (geometries, vector) = _command.GetInputValues();

            var transform = new Projection(new Plane(Point3d.Origin, vector, Vector3d.ZAxis)).ToMatrix();

            var projected = new List<InitialGeometryWithProjection>();
            for (int i = 0; i < geometries.Count; i++)
            {
                var transformed = geometries[i].DuplicateGeometry().Transform(transform);
                var line = new Line(transformed.Boundingbox.Min, transformed.Boundingbox.Max);
                projected.Add(new InitialGeometryWithProjection(geometries[i], line));
            }

            var perpPlane = new Plane(projected.First().Projection.From, vector);
            var ordered = projected.OrderBy(p => p.DistanceTo(perpPlane));

            var orderedGeometries = ordered.Select(o => o.Geometry).ToList();
            var indicies = new List<int>();
            for (int i = 0; i < geometries.Count; i++)
                indicies.Add(orderedGeometries.IndexOf(geometries[i]));

            _command.SetOutputValues(DA, orderedGeometries, indicies);
        }

        private class InitialGeometryWithProjection
        {
            public IGH_GeometricGoo Geometry { get; }
            public Line Projection { get; }

            public InitialGeometryWithProjection(IGH_GeometricGoo geometry, Line projection)
            {
                Geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
                Projection = projection;
            }

            public double DistanceTo(Plane plane)
            {
                var distFrom = plane.DistanceTo(Projection.From);
                var distTo = plane.DistanceTo(Projection.To);

                return Math.Min(distTo, distFrom);
            }
        }
    }

    public class SortByVectorCommand : CommandBase
    {
        private readonly InputListParam<IGH_GeometricGoo> _inGeometries = new InputListParam<IGH_GeometricGoo>(ParamInfos.Geometry);
        private readonly InputStructParam<Vector3d> _inDirection = new InputStructParam<Vector3d>(ParamInfos.DirectionVector);

        private readonly OutputListParam<IGH_GeometricGoo> _outGeometries = new OutputListParam<IGH_GeometricGoo>(ParamInfos.Geometry);
        private readonly OutputListParam<int> _outIndicies = new OutputListParam<int>(ParamInfos.OrderIndicies);

        internal (List<IGH_GeometricGoo>? InputGeometries, Vector3d Direction) GetInputValues()
        {
            return (_inGeometries.Value,
                    _inDirection.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<IGH_GeometricGoo> geometries, List<int> indicies)
        {
            _outGeometries.Value = geometries;
            _outIndicies.Value = indicies;

            return SetOutput(DA);
        }
    }
}
