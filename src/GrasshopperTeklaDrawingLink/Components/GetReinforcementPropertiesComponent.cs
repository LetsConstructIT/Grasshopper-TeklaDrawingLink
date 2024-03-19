using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class GetReinforcementPropertiesComponent : TeklaComponentBaseNew<GetReinforcementPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.ReinforcementProperties;

        public GetReinforcementPropertiesComponent() : base(ComponentInfos.GetReinforcementPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var reinforcement = _command.GetInputValues();
            if (reinforcement is null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input object could not be casted to Reinforcment");
                return;
            }

            var size = string.Empty;
            reinforcement.GetReportProperty("SIZE", ref size);
            var shape = string.Empty;
            reinforcement.GetReportProperty("SHAPE", ref shape);

            var (geometries, radiuses) = GetRebarGeometries(reinforcement);

            _command.SetOutputValues(DA,
                                     size,
                                     reinforcement.Grade,
                                     reinforcement.Name,
                                     GetRebarType(reinforcement),
                                     shape,
                                     geometries,
                                     radiuses);
        }

        private string GetRebarType(TSM.Reinforcement reinforcement)
        {
            var type = reinforcement.GetType().Name;

            return type.Replace("Tekla.Structures.Model.", "");
        }


#if API2020
        private (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.Reinforcement reinforcement)
        {
            var rebarPolylines = new List<Rhino.Geometry.Polyline>();
            var bendingRadiuses = new List<double>();

            var geometries = reinforcement.GetRebarGeometriesWithoutClashes(true);
            foreach (TSM.RebarGeometry geometry in geometries)
            {
                var polyline = new Rhino.Geometry.Polyline();

                foreach (TSG.Point point in geometry.Shape.Points)
                {
                    polyline.Add(point.ToRhino());
                }

                rebarPolylines.Add(polyline);

                if (bendingRadiuses.Count == 0)
                {
                    foreach (double radius in geometry.BendingRadiuses)
                        bendingRadiuses.Add(radius);
                }
            }

            return (rebarPolylines, bendingRadiuses);
        }

#else
        private (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.Reinforcement reinforcement)
        {
            var rebarPolylines = new List<Rhino.Geometry.Polyline>();
            var bendingRadiuses = new List<double>();

            var geometries = reinforcement.GetRebarComplexGeometries(true, true, true);
            foreach (var geometry in geometries)
            {
                var polyline = new Rhino.Geometry.Polyline();

                TSG.LineSegment lastSegment = null;
                foreach (var rebarLeg in geometry.Legs)
                {
                    if (rebarLeg.Curve is TSG.LineSegment segment)
                    {
                        polyline.Add(segment.Point1.ToRhino());
                        lastSegment = segment;
                    }
                }

                if (lastSegment != null)
                    polyline.Add(lastSegment.Point2.ToRhino());

                rebarPolylines.Add(polyline);

                if (bendingRadiuses.Count == 0)
                    bendingRadiuses = geometry.BendingRadiuses;
            }

            return (rebarPolylines, bendingRadiuses);
        }
#endif

    }

    public class GetReinforcementPropertiesCommand : CommandBase
    {
        private readonly InputParam<TSM.ModelObject> _inTeklaObject = new InputParam<TSM.ModelObject>(ParamInfos.ReinforcementObject);

        private readonly OutputParam<string> _outSize = new OutputParam<string>(ParamInfos.ReinforcementSize);
        private readonly OutputParam<string> _outGrade = new OutputParam<string>(ParamInfos.ReinforcementGrade);
        private readonly OutputParam<string> _outName = new OutputParam<string>(ParamInfos.ReinforcementName);
        private readonly OutputParam<string> _outType = new OutputParam<string>(ParamInfos.ReinforcementType);
        private readonly OutputParam<string> _outShape = new OutputParam<string>(ParamInfos.ReinforcementShape);
        private readonly OutputListParam<Rhino.Geometry.Polyline> _outGeometry = new OutputListParam<Rhino.Geometry.Polyline>(ParamInfos.ReinforcementGeometry);
        private readonly OutputListParam<double> _outBendingRadiuses = new OutputListParam<double>(ParamInfos.ReinforcementBendingRadius);

        internal TSM.Reinforcement GetInputValues()
        {
            return GetReinforcementFromInput(_inTeklaObject.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, string size, string grade, string name, string type, string shape, List<Rhino.Geometry.Polyline> geometries, List<double> bendingRadiuses)
        {
            _outSize.Value = size;
            _outGrade.Value = grade;
            _outName.Value = name;
            _outType.Value = type;
            _outShape.Value = shape;
            _outGeometry.Value = geometries;
            _outBendingRadiuses.Value = bendingRadiuses;

            return SetOutput(DA);
        }

        private TSM.Reinforcement? GetReinforcementFromInput(object inputObject)
        {
            if (inputObject is GH_Goo<TSM.ModelObject> modelGoo)
            {
                return (modelGoo.Value) as TSM.Reinforcement;
            }
            else if (inputObject is TSM.ModelObject modelObject)
            {
                return modelObject as TSM.Reinforcement;
            }
            else if (inputObject is TeklaDatabaseObjectGoo drawingObject)
            {
                var drawingModelObject = drawingObject.Value as ModelObject;
                if (drawingModelObject is null)
                    return null;

                return ModelInteractor.GetModelObject(drawingModelObject.ModelIdentifier) as TSM.Reinforcement;
            }

            return null;
        }
    }
}
