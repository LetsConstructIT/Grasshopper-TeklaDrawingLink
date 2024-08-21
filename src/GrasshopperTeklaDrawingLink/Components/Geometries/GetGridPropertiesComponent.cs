using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Geometries
{
    public class GetGridPropertiesComponent : TeklaComponentBaseNew<GetGridPropertiesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GridProperties;
        public GetGridPropertiesComponent() : base(ComponentInfos.GetGridPropertiesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var input = _command.GetInputValues();

            if (!(input is GridBase gridBase))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided object is not a Tekla Grid");
                return;
            }

            var surfaces = new List<Surface>();
            var labels = new List<string>();

            var gridMoe = gridBase.GetChildren();
            while (gridMoe.MoveNext())
            {
                var gridSurface = gridMoe.Current as GridSurface;
                labels.Add(gridSurface.Label);

                if (gridSurface is GridPlane gridPlane)
                {
                    var plane = gridPlane.Plane;

                    var xInterval = new Interval(0, plane.AxisX.GetLength());
                    var yInterval = new Interval(0, plane.AxisY.GetLength());

                    var rhinoPlane = new Rhino.Geometry.Plane(plane.Origin.ToRhino(), plane.AxisX.GetNormal().ToRhino(), plane.AxisY.GetNormal().ToRhino());
                    surfaces.Add(new PlaneSurface(rhinoPlane, xInterval, yInterval));
                }
                else if (gridSurface is GridCylindricalSurface cylindricalSurface)
                {
                    var rhinoArc = cylindricalSurface.CylinderBase.ToRhino();
                    var direction = cylindricalSurface.CylinderBase.Normal;
                    direction *= cylindricalSurface.CylinderHeight;

                    surfaces.Add(Surface.CreateExtrusion(rhinoArc.ToNurbsCurve(), direction.ToRhino()));
                }
            }
            var xySurfaces = new List<Surface>();
            var xyLabels = new List<string>();

            var notXYSurfaces = new List<Surface>();
            var notXYLabels = new List<string>();
            for (int i = 0; i < surfaces.Count; i++)
            {
                var surface = surfaces[i];
                var label = labels[i];
                var normal = surface.NormalAt(0.5, 0.5);
                var dot = normal * Vector3d.ZAxis;
                if (Math.Abs(dot) >= 0.99)
                {
                    xySurfaces.Add(surface);
                    xyLabels.Add(label);
                }
                else
                {
                    notXYSurfaces.Add(surface);
                    notXYLabels.Add(label);
                }
            }

            _command.SetOutputValues(DA, surfaces, labels, xySurfaces, xyLabels, notXYSurfaces, notXYLabels);
        }
    }

    public class GetGridPropertiesCommand : CommandBase
    {
        private readonly InputParam<ModelObject> _inGrid = new InputParam<ModelObject>(ParamInfos.Grid);

        private readonly OutputListParam<Surface> _outSurfaces = new OutputListParam<Surface>(ParamInfos.GridSurface);
        private readonly OutputListParam<string> _outLabels = new OutputListParam<string>(ParamInfos.GridLabel);

        private readonly OutputListParam<Surface> _outXYSurfaces = new OutputListParam<Surface>(ParamInfos.GridXYSurface);
        private readonly OutputListParam<string> _outXYLabels = new OutputListParam<string>(ParamInfos.GridXYLabel);

        private readonly OutputListParam<Surface> _outNotXYSurfaces = new OutputListParam<Surface>(ParamInfos.GridNotXYSurface);
        private readonly OutputListParam<string> _outNotXYLabels = new OutputListParam<string>(ParamInfos.GridNotXYLabel);
        internal ModelObject GetInputValues()
        {
            return _inGrid.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA,
            List<Surface> surfaces, List<string> labels,
            List<Surface> xySurfaces, List<string> xyLabels,
            List<Surface> notXYSurfaces, List<string> notXYLabels)
        {
            _outSurfaces.Value = surfaces;
            _outLabels.Value = labels;
            _outXYSurfaces.Value = xySurfaces;
            _outXYLabels.Value = xyLabels;
            _outNotXYSurfaces.Value = notXYSurfaces;
            _outNotXYLabels.Value = notXYLabels;

            return SetOutput(DA);
        }
    }
}
