using Grasshopper.Kernel;
using System.Collections.Generic;

namespace DrawingLink.UI.GH
{
    public class TeklaModelParam : TeklaParamBase
    {
        public ModelParamType ParamType { get; }

        public TeklaModelParam(IGH_ActiveObject activeObject, ModelParamType paramType, bool isMultiple, string prompt) : base(activeObject, isMultiple, prompt)
        {
            ParamType = paramType;

            TeklaObjects = new TeklaObjects(isDrawing: false);
            TeklaObjects.SetModelType(ParamType);
        }

        public void Set(List<Tekla.Structures.Model.ModelObject> modelObjects)
        {
            TeklaObjects.Set(modelObjects);
        }

        public void Set(List<Tekla.Structures.Geometry3d.Point> points)
        {
            TeklaObjects.Set(points);
        }
    }
}
