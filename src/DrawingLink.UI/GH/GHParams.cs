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
        public IReadOnlyList<IGH_ActiveObject> ModelParams { get; }
        public IReadOnlyList<IGH_ActiveObject> DrawingParams { get; }
        public IReadOnlyList<ActiveObjectWrapper> AttributeParams { get; }

        public GHParams(List<IGH_ActiveObject> modelParams, List<IGH_ActiveObject> drawingParams, List<ActiveObjectWrapper> attributeParams)
        {
            ModelParams = modelParams ?? throw new ArgumentNullException(nameof(modelParams));
            DrawingParams = drawingParams ?? throw new ArgumentNullException(nameof(drawingParams));
            AttributeParams = attributeParams ?? throw new ArgumentNullException(nameof(attributeParams));

            ApplyFieldNames();
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
}
