using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;

namespace DrawingLink.UI.GH
{
    public class GHParams
    {
        public TeklaParams TeklaParams { get; }
        public IReadOnlyList<ActiveObjectWrapper> AttributeParams { get; }

        public GHParams(TeklaParams teklaParams, List<ActiveObjectWrapper> attributeParams)
        {
            TeklaParams = teklaParams ?? throw new ArgumentNullException(nameof(teklaParams));
            AttributeParams = attributeParams ?? throw new ArgumentNullException(nameof(attributeParams));

            ApplyFieldNames();
            ApplyTeklaFieldNames();
        }

        private void ApplyFieldNames()
        {
            var stringCounter = 0;
            var doubleCounter = 0;
            var intCounter = 0;

            foreach (var activeObjectWrapper in AttributeParams)
            {
                var fieldName = activeObjectWrapper.ActiveObject switch
                {
                    Param_FilePath => GetNextStringFieldName(),
                    GH_PersistentParam<GH_String> => GetNextStringFieldName(),
                    GH_PersistentParam<GH_Integer> => GetNextIntFieldName(),
                    GH_PersistentParam<GH_Number> => GetNextDoubleFieldName(),
                    GH_Panel panel => activeObjectWrapper.Connectivity.IsStandalone() ? "" : GetNextStringFieldName(),
                    GH_ValueList => GetNextStringFieldName(),
                    GH_BooleanToggle => GetNextStringFieldName(),
                    GH_ButtonObject => GetNextStringFieldName(),
                    GH_NumberSlider => GetNextDoubleFieldName(),
                    _ => IsCatalogBaseComponent(activeObjectWrapper.ActiveObject) ? GetNextStringFieldName() : ""
                };

                activeObjectWrapper.FieldName = fieldName;
            }

            string GetNextStringFieldName()
                => $"string_{stringCounter++}";
            string GetNextDoubleFieldName()
                => $"double_{doubleCounter++}";
            string GetNextIntFieldName()
                => $"int_{intCounter++}";
            bool IsCatalogBaseComponent(IGH_ActiveObject activeObject)
                => activeObject.GetType().BaseType.Name == "CatalogBaseComponent";
        }

        private void ApplyTeklaFieldNames()
        {
            var counter = 0;
            foreach (var param in TeklaParams.ModelParams)
                param.FieldName = $"tekla_{counter++}";

            foreach (var param in TeklaParams.DrawingParams)
                param.FieldName = $"tekla_{counter++}";
        }
    }

    public class ActiveObjectWrapper
    {
        public IGH_ActiveObject ActiveObject { get; }
        public TabAndGroup TabAndGroup { get; }
        public ObjectConnectivity Connectivity { get; }
        public string FieldName { get; set; }

        public ActiveObjectWrapper(IGH_ActiveObject activeObject, TabAndGroup tabAndGroup, ObjectConnectivity connectivity)
        {
            ActiveObject = activeObject ?? throw new ArgumentNullException(nameof(activeObject));
            TabAndGroup = tabAndGroup ?? throw new ArgumentNullException(nameof(tabAndGroup));
            Connectivity = connectivity ?? throw new ArgumentNullException(nameof(connectivity));
            FieldName = string.Empty;
        }
    }

    public abstract class TeklaParamBase
    {
        public IGH_ActiveObject ActiveObject { get; }
        public string FieldName { get; set; }
        public bool IsMultiple { get; }
        public string Prompt { get; }

        public TeklaObjects TeklaObjets { get; }

        protected TeklaParamBase(IGH_ActiveObject activeObject, bool isMultiple, string prompt)
        {
            ActiveObject = activeObject ?? throw new ArgumentNullException(nameof(activeObject));
            IsMultiple = isMultiple;
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            FieldName = string.Empty;

            TeklaObjets = new TeklaObjects();
        }
    }

    public class TeklaModelParam : TeklaParamBase
    {
        public ModelParamType ParamType { get; }

        public TeklaModelParam(IGH_ActiveObject activeObject, ModelParamType paramType, bool isMultiple, string prompt) : base(activeObject, isMultiple, prompt)
        {
            ParamType = paramType;
        }

        public void Set(List<Tekla.Structures.Model.ModelObject> modelObjects)
        {
            TeklaObjets.Set(modelObjects);
        }

        public void Set(List<Tekla.Structures.Geometry3d.Point> points)
        {
            TeklaObjets.Set(points);
        }
    }

    public class TeklaObjects
    {
        private List<Tekla.Structures.Model.ModelObject>? _modelObjects;
        private List<Tekla.Structures.Drawing.DatabaseObject>? _drawingObjects;
        private List<Tekla.Structures.Geometry3d.Point>? _points;

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

    public class TeklaDrawingParam : TeklaParamBase
    {
        public DrawingParamType ParamType { get; }

        private List<Tekla.Structures.Drawing.DatabaseObject>? _drawingObjects;
        private List<Tekla.Structures.Geometry3d.Point>? _points;

        public TeklaDrawingParam(IGH_ActiveObject activeObject, DrawingParamType paramType, bool isMultiple, string prompt) : base(activeObject, isMultiple, prompt)
        {
            ParamType = paramType;
        }

        public void Set(List<Tekla.Structures.Drawing.DatabaseObject> drawingObjects)
        {
            TeklaObjets.Set(drawingObjects);
        }

        public void Set(List<Tekla.Structures.Geometry3d.Point> points)
        {
            TeklaObjets.Set(points);
        }
    }

    public enum ModelParamType
    {
        Point,
        Line,
        Polyline,
        Face,
        Object
    }

    public enum DrawingParamType
    {
        Point,
        Object
    }
}
