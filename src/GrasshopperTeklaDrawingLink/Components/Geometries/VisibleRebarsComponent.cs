using Grasshopper.Kernel;
using GTDrawingLink.Components.Parts;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Geometries
{
    public class VisibleRebarsComponent : TeklaComponentBaseNew<VisibleRebarsCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.VisibleRebars;

        public VisibleRebarsComponent() : base(ComponentInfos.VisibleRebarsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var drawingRebar = _command.GetInputValues();
            drawingRebar.Select();

            var fatherView = drawingRebar.GetView();
            if (!(fatherView is View view))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided rebar does not have valid father view");
                return;
            }

            var modelRebar = ConvertDrawingToModelObjectComponent.ConvertToModelRebar(drawingRebar);
            var (geometries, radiuses) = GetReinforcementPropertiesComponent.GetRebarGeometries(modelRebar);

            var visibilityType = GetVisiblityType(drawingRebar);
            var customPosition = GetCustomPosition(drawingRebar);

            var visibleGeometries = PickOnlyVisible(geometries, visibilityType, customPosition);
            var localGeometries = Transform(visibleGeometries, view.DisplayCoordinateSystem);

            _command.SetOutputValues(DA,
                                     localGeometries,
                                     radiuses);
        }

        private List<Rhino.Geometry.Polyline> PickOnlyVisible(List<Rhino.Geometry.Polyline> geometries, ReinforcementBase.ReinforcementVisibilityTypes visibilityType, double customPosition)
        {
            return visibilityType switch
            {
                ReinforcementBase.ReinforcementVisibilityTypes.First => new List<Rhino.Geometry.Polyline>() { geometries.First() },
                ReinforcementBase.ReinforcementVisibilityTypes.Last => new List<Rhino.Geometry.Polyline>() { geometries.Last() },
                ReinforcementBase.ReinforcementVisibilityTypes.FirstAndLast => new List<Rhino.Geometry.Polyline>() { geometries.First(), geometries.Last() },
                ReinforcementBase.ReinforcementVisibilityTypes.OneInTheMiddle => PickOneInTheMiddle(),
                ReinforcementBase.ReinforcementVisibilityTypes.TwoInTheMiddle => PickTwoInTheMiddle(),
                _ => geometries
            };

            List<Rhino.Geometry.Polyline> PickOneInTheMiddle()
            {
                var index = GetMidIndex(geometries);
                return new List<Rhino.Geometry.Polyline>() { geometries[index] };
            }

            List<Rhino.Geometry.Polyline> PickTwoInTheMiddle()
            {
                var index = GetMidIndex(geometries);
                var result = new List<Rhino.Geometry.Polyline>() { geometries[index] };
                if (geometries.Count > 1)
                    result.Add(geometries[index + 1]);

                return result;
            }

            int GetMidIndex(List<Rhino.Geometry.Polyline> geometries)
            {
                return (int)(geometries.Count * 0.35);
            }
        }


        private List<Rhino.Geometry.Polyline> Transform(List<Rhino.Geometry.Polyline> geometries, CoordinateSystem displayCoordinateSystem)
        {
            var matrix = MatrixFactory.ToCoordinateSystem(displayCoordinateSystem);
            var result = new List<Rhino.Geometry.Polyline>();
            foreach (var geometry in geometries)
            {
                var localPts = geometry.Select(p => matrix.Transform(p.ToTekla()).ToRhino());
                result.Add(new Rhino.Geometry.Polyline(localPts));
            }

            return result;
        }

        private static ReinforcementBase.ReinforcementVisibilityTypes GetVisiblityType(ReinforcementBase drawingRebar)
        {
            var visibilityType = ReinforcementBase.ReinforcementVisibilityTypes.All;
            if (drawingRebar is ReinforcementSetGroup rebarSet)
                visibilityType = rebarSet.Attributes.ReinforcementVisibility;
            else if (drawingRebar is ReinforcementStrand strand)
                visibilityType = strand.Attributes.ReinforcementVisibility;
            else if (drawingRebar is ReinforcementGroup group)
                visibilityType = group.Attributes.ReinforcementVisibility;
            else if (drawingRebar is ReinforcementMesh mesh)
                visibilityType = mesh.Attributes.MeshReinforcementVisibilityLongitudinal;

            return visibilityType;
        }

        private static double GetCustomPosition(ReinforcementBase drawingRebar)
        {
            var customPosition = 1.0;
            if (drawingRebar is ReinforcementSetGroup rebarSet)
                customPosition = rebarSet.ReinforcementCustomPosition;
            else if (drawingRebar is ReinforcementStrand strand)
                customPosition = strand.ReinforcementCustomPosition;
            else if (drawingRebar is ReinforcementGroup group)
                customPosition = group.ReinforcementCustomPosition;
            else if (drawingRebar is ReinforcementMesh mesh)
                customPosition = mesh.ReinforcementCustomPositionLongitudinal;

            return customPosition;
        }
    }

    public class VisibleRebarsCommand : CommandBase
    {
        private readonly InputParam<ReinforcementBase> _inReinforcementObject = new InputParam<ReinforcementBase>(ParamInfos.DrawingReinforcementObject);

        private readonly OutputListParam<Rhino.Geometry.Polyline> _outGeometry = new OutputListParam<Rhino.Geometry.Polyline>(ParamInfos.ReinforcementGeometry);
        private readonly OutputListParam<double> _outBendingRadiuses = new OutputListParam<double>(ParamInfos.ReinforcementBendingRadius);
        internal ReinforcementBase GetInputValues()
        {
            return _inReinforcementObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Rhino.Geometry.Polyline> geometries, List<double> bendingRadiuses)
        {
            _outGeometry.Value = geometries;
            _outBendingRadiuses.Value = bendingRadiuses;

            return SetOutput(DA);
        }
    }
}
