using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Types.Transforms;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components
{
    public class SimpleOrientComponent : TeklaComponentBaseNew<SimpleOrientCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SimpleOrient;
        public SimpleOrientComponent() : base(ComponentInfos.SimpleOrientComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (geometries, planes) = _command.GetInputValues();

            var transformations = planes.Select(p => new Orientation(p, Plane.WorldXY).ToMatrix()).ToList();

            var output = new GH_Structure<IGH_GeometricGoo>();
            for (int i = 0; i < geometries.PathCount; i++)
            {
                var objects = geometries.Objects[i];
                var path = geometries.Paths[i];

                var index = path.Indices.First();
                if (transformations.Count <= index)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Index {index} does not exist in specified list of planes");
                    break;
                }

                var transformation = transformations[index];
                foreach (var geometricGoo in objects)
                {
                    var transformed = geometricGoo.DuplicateGeometry().Transform(transformation);
                    output.Append(transformed, path);
                }
            }

            _command.SetOutputValues(DA, output);
        }
    }

    public class SimpleOrientCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_GeometricGoo> _inGeometries = new InputTreeParam<IGH_GeometricGoo>(ParamInfos.Geometry);
        private readonly InputStructListParam<Plane> _inPlanes = new InputStructListParam<Plane>(ParamInfos.Plane);

        private readonly OutputTreeParam<IGH_GeometricGoo> _outGeometries = new OutputTreeParam<IGH_GeometricGoo>(ParamInfos.Geometry, 0);

        internal (TreeData<IGH_GeometricGoo> InputGeometries, List<Plane> Plane) GetInputValues()
        {
            return (_inGeometries.AsTreeData(),
                    _inPlanes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure geometries)
        {
            _outGeometries.Value = geometries;

            return SetOutput(DA);
        }
    }
}
