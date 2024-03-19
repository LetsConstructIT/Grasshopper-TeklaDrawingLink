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
    public class GetBoltPropertiesComponent : TeklaComponentBaseNew<GetBoltPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.BoltProperties;

        public GetBoltPropertiesComponent() : base(ComponentInfos.GetBoltPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var bolt = _command.GetInputValues();
            if (bolt is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input object could not be casted to Bolt");
                return;
            }

            var boltPositions = GetBoltPositions(bolt.BoltPositions);
            var direction = GetZDirection(bolt.GetCoordinateSystem());

            _command.SetOutputValues(DA,
                                     bolt.BoltSize,
                                     bolt.BoltStandard,
                                     bolt.BoltType.ToString(),
                                     boltPositions,
                                     direction);
        }

        private List<Point3d> GetBoltPositions(ArrayList boltPositions)
        {
            var points = new List<Point3d>();
            foreach (TSG.Point point in boltPositions)
            {
                points.Add(point.ToRhino());
            }

            return points;
        }

        private Vector3d GetZDirection(CoordinateSystem coordinateSystem)
        {
            var zDir = coordinateSystem.AxisX.Cross(coordinateSystem.AxisY).GetNormal();

            return zDir.ToRhino();
        }
    }

    public class GetBoltPropertiesCommand : CommandBase
    {
        private readonly InputParam<TSM.ModelObject> _inTeklaObject = new InputParam<TSM.ModelObject>(ParamInfos.BoltObject);

        private readonly OutputParam<double> _outSize = new OutputParam<double>(ParamInfos.BoltSize);
        private readonly OutputParam<string> _outStandard = new OutputParam<string>(ParamInfos.BoltStandard);
        private readonly OutputParam<string> _outType = new OutputParam<string>(ParamInfos.BoltType);
        private readonly OutputListParam<Point3d> _outPositions = new OutputListParam<Point3d>(ParamInfos.BoltPositions);
        private readonly OutputParam<Vector3d> _outDirection = new OutputParam<Vector3d>(ParamInfos.BoltDirection);

        internal TSM.BoltGroup? GetInputValues()
        {
            return GetBoltFromInput(_inTeklaObject.Value);
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

        private TSM.BoltGroup? GetBoltFromInput(object inputObject)
        {
            if (inputObject is GH_Goo<TSM.ModelObject> modelGoo)
            {
                return (modelGoo.Value) as TSM.BoltGroup;
            }
            else if (inputObject is TSM.ModelObject modelObject)
            {
                return modelObject as TSM.BoltGroup;
            }
            else if (inputObject is TeklaDatabaseObjectGoo drawingObject)
            {
                var drawingModelObject = drawingObject.Value as ModelObject;
                if (drawingModelObject is null)
                    return null;

                return ModelInteractor.GetModelObject(drawingModelObject.ModelIdentifier) as TSM.BoltGroup;
            }

            return null;
        }
    }
}
