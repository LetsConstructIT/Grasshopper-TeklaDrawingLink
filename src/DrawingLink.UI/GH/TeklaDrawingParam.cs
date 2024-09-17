using Grasshopper.Kernel;
using System.Collections.Generic;

namespace DrawingLink.UI.GH
{
    public class TeklaDrawingParam : TeklaParamBase
    {
        public DrawingParamType ParamType { get; }

        private List<Tekla.Structures.Drawing.DatabaseObject>? _drawingObjects;
        private List<Tekla.Structures.Geometry3d.Point>? _points;

        public TeklaDrawingParam(IGH_ActiveObject activeObject, DrawingParamType paramType, bool isMultiple, string prompt) : base(activeObject, isMultiple, prompt)
        {
            ParamType = paramType;

            TeklaObjects = new TeklaObjects(isDrawing: true);
            TeklaObjects.SetDrawingType(ParamType);
        }

        public void Set(List<Tekla.Structures.Drawing.DatabaseObject> drawingObjects)
        {
            TeklaObjects.Set(drawingObjects);
        }

        public void Set(List<Tekla.Structures.Geometry3d.Point> points)
        {
            TeklaObjects.Set(points);
        }
    }
}
