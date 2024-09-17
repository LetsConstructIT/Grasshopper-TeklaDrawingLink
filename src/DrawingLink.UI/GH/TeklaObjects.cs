using System.Collections.Generic;
using TSG = Tekla.Structures.Geometry3d;

namespace DrawingLink.UI.GH
{
    public class TeklaObjects
    {
        private List<Tekla.Structures.Model.ModelObject>? _modelObjects;
        private List<Tekla.Structures.Drawing.DatabaseObject>? _drawingObjects;
        private List<Tekla.Structures.Geometry3d.Point>? _points;

        private readonly bool _isDrawing;
        private DrawingParamType _drawingType;
        private ModelParamType _modelType;

        public TeklaObjects(bool isDrawing)
        {
            _isDrawing = isDrawing;
        }

        public void SetDrawingType(DrawingParamType drawingParamType)
        {
            _drawingType = drawingParamType;
        }

        public void SetModelType(ModelParamType modelParamType)
        {
            _modelType = modelParamType;
        }

        public object[] GetCorrectObject()
        {
            if (_isDrawing)
            {
                return _drawingType == DrawingParamType.Point ? _points.ToArray() : _drawingObjects.ToArray();
            }
            else
            {
                return _modelType switch
                {
                    ModelParamType.Point => _points.ToArray(),
                    ModelParamType.Line => new object[] { new TSG.LineSegment(_points[0], _points[1]) },
                    ModelParamType.Polyline => new object[] { new TSG.PolyLine(_points) },
                    ModelParamType.Face => new object[] { new TSG.PolyLine(_points) },
                    _ => _modelObjects.ToArray(),
                };
            }
        }

        internal void Set(List<Tekla.Structures.Model.ModelObject> modelObjects)
        {
            _modelObjects = modelObjects;
        }

        public void Set(List<Tekla.Structures.Geometry3d.Point> points)
        {
            _points = points;
        }

        public void Set(List<Tekla.Structures.Drawing.DatabaseObject> drawingObjects)
        {
            _drawingObjects = drawingObjects;
        }
    }
}
