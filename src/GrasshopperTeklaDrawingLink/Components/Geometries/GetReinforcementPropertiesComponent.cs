using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Components.Parts;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components.Geometries
{
    public class GetReinforcementPropertiesComponent : TeklaComponentBaseNew<GetReinforcementPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.ReinforcementProperties;

        public GetReinforcementPropertiesComponent() : base(ComponentInfos.GetReinforcementPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var reinforcement = _command.GetInputValues();
            if (!CheckInput(reinforcement))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input object could not be casted to Reinforcment or Rebar Set");
                return;
            }

            var size = string.Empty;
            reinforcement.GetReportProperty("SIZE", ref size);

            var shape = GetShape(reinforcement);

            var grade = string.Empty;
            var name = string.Empty;
            if (reinforcement is TSM.Reinforcement rebar)
            {
                grade = rebar.Grade;
                name = rebar.Name;
            }
            else if (reinforcement is TSM.RebarSet rebarSet)
            {
                grade = rebarSet.RebarProperties.Grade;
                name = rebarSet.RebarProperties.Name;
                size = rebarSet.RebarProperties.Size;
            }

            var (geometries, radiuses) = GetRebarGeometries(reinforcement);

            _command.SetOutputValues(DA,
                                     size,
                                     grade,
                                     name,
                                     GetRebarType(reinforcement),
                                     shape,
                                     geometries,
                                     radiuses);
        }

        private bool CheckInput(TSM.ModelObject reinforcement)
        {
            if (reinforcement is null)
                return false;

            if (!(reinforcement is TSM.Reinforcement) && !(reinforcement is TSM.RebarSet))
            {
                return false;
            }

            return true;
        }

        private string GetShape(TSM.ModelObject reinforcement)
        {
            var shape = string.Empty;
            reinforcement.GetReportProperty("SHAPE", ref shape);
            if (reinforcement is TSM.RebarSet rebarSet)
            {
                var childRebar = GetFirstRebar(rebarSet);
                childRebar.GetReportProperty("SHAPE", ref shape);
            }

            return shape;
        }

        private string GetRebarType(TSM.ModelObject reinforcement)
        {
            var type = reinforcement.GetType().Name;

            return type.Replace("Tekla.Structures.Model.", "");
        }

        private (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.ModelObject input)
        {
            if (input is TSM.RebarSet rebarSet)
                return GetRebarGeometries(rebarSet);

            return GetRebarGeometries(input as TSM.Reinforcement);
        }

#if API2020
        public static (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.Reinforcement reinforcement)
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
        public static (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.Reinforcement reinforcement)
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

        private (List<Rhino.Geometry.Polyline> geometries, List<double> radiuses) GetRebarGeometries(TSM.RebarSet rebarSet)
        {
            var childRebar = GetFirstRebar(rebarSet);

            return GetRebarGeometries(childRebar);
        }

        private TSM.Reinforcement GetFirstRebar(TSM.RebarSet rebarSet)
        {
            var moe = rebarSet.GetReinforcements();
            moe.MoveNext();

            return moe.Current as TSM.Reinforcement;
        }
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

        internal TSM.ModelObject? GetInputValues()
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

        private TSM.ModelObject? GetReinforcementFromInput(object inputObject)
        {
            if (inputObject is GH_Goo<TSM.ModelObject> modelGoo)
            {
                return modelGoo.Value;
            }
            else if (inputObject is TSM.ModelObject modelObject)
            {
                return modelObject;
            }
            else if (inputObject is TeklaDatabaseObjectGoo drawingObject)
            {
                if (drawingObject.Value is ReinforcementBase drawingRebar)
                    return ConvertDrawingToModelObjectComponent.ConvertToModelRebar(drawingRebar);
            }

            return null;
        }
    }
}
