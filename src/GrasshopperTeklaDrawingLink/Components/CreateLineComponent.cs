using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class CreateLineComponent : TeklaComponentBaseNew<CreateLineCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.Line;

        public CreateLineComponent() : base(ComponentInfos.CreateLineComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var test = _command.GetInputValues();

            //_command.SetOutputValues(DA,
            //                         bolt.BoltSize,
            //                         bolt.BoltStandard,
            //                         bolt.BoltType.ToString(),
            //                         boltPositions,
            //                         direction);
        }
    }

    public class CreateLineCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_GeometricGoo> _inGeometricGoo = new InputTreeParam<IGH_GeometricGoo>(ParamInfos.Curve);

        private readonly OutputParam<double> _outSize = new OutputParam<double>(ParamInfos.BoltSize);
        private readonly OutputParam<string> _outStandard = new OutputParam<string>(ParamInfos.BoltStandard);
        private readonly OutputParam<string> _outType = new OutputParam<string>(ParamInfos.BoltType);
        private readonly OutputListParam<Point3d> _outPositions = new OutputListParam<Point3d>(ParamInfos.BoltPositions);
        private readonly OutputParam<Vector3d> _outDirection = new OutputParam<Vector3d>(ParamInfos.BoltDirection);

        internal List<List<TSG.Point>> GetInputValues()
        {
            return ParseInputTree(_inGeometricGoo.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, double size, string standard, string type, List<Point3d> positions, Vector3d direction)
        {
            _outSize.Value = size;
            _outStandard.Value = standard;
            _outType.Value = type;
            _outPositions.Value = positions;
            _outDirection.Value = direction;

            return SetOutput(DA);
        }

        private List<List<TSG.Point>> ParseInputTree(List<List<IGH_GeometricGoo>> inputObject)
        {
            var parsed = new List<List<TSG.Point>>();


            return parsed;
        }
    }
}
