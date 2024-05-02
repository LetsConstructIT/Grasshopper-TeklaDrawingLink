using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;

namespace GTDrawingLink.Components.Geometries
{
    public class SortByVectorComponent : TeklaComponentBaseNew<SortByVectorCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SortByVector;
        public SortByVectorComponent() : base(ComponentInfos.SortByVectorComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (geometries, vector) = _command.GetInputValues();

            var orderedGeometries = new GeometrySorter().OrderGeometries(geometries, vector);

            var indicies = new List<int>();
            for (int i = 0; i < geometries.Count; i++)
                indicies.Add(orderedGeometries.IndexOf(geometries[i]));

            _command.SetOutputValues(DA, orderedGeometries, indicies);
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
