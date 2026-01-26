using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tekla.Structures.Filtering;
using Tekla.Structures.TeklaStructuresInternal.Filtering;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Components.Miscs
{
    public class ObjectMatchesToFilterComponent : TeklaComponentBaseNew<ObjectMatchesToFilterCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.ObjectMatchesToFilter;
        public ObjectMatchesToFilterComponent() : base(ComponentInfos.ObjectMatchesToFilterComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<TSM.ModelObject> modelObjects, string filterName) = _command.GetInputValues();
            if (modelObjects.Any(o => o is null))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One of the provided model objects does not exist");
                return;
            }

            filterName = CreateDynamicFilterIfNeeded(filterName);

            var filtered = new List<TSM.ModelObject>();
            var pattern = new List<bool>();

            foreach (var modelObject in modelObjects)
            {
                var isMatch = TSM.Operations.Operation.ObjectMatchesToFilter(modelObject, filterName);
                pattern.Add(isMatch);

                if (isMatch)
                    filtered.Add(modelObject);
            }

            _command.SetOutputValues(DA, filtered, pattern);
        }

        private string CreateDynamicFilterIfNeeded(string filterName)
        {
            var dynamicParser = EstablishParserType(filterName);
            if (dynamicParser is null)
                return filterName;

            var errorMessage = string.Empty;
            var filterExpression = FilterHelper.Parse(filterName, dynamicParser, ref errorMessage, true);
            if (!string.IsNullOrEmpty(errorMessage))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, errorMessage);

            filterName = "[TemporaryFilter]";
            var tempFilterFileName = Path.Combine(Path.Combine(ModelInteractor.ModelPath(), "attributes"), filterName);
            tempFilterFileName = FilterHelper.CreateFilter(filterExpression, tempFilterFileName, ref errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, errorMessage);
            
            return Path.GetFileNameWithoutExtension(tempFilterFileName);
        }

        private FilterExpressionParserType? EstablishParserType(string filterName)
        {
            if (filterName.StartsWith("("))
                return new FilterExpressionParserType?(FilterExpressionParserType.C);
            else if (filterName.Contains("\n"))
                return new FilterExpressionParserType?(FilterExpressionParserType.TEKLA);

            return null;
        }

    }

    public class ObjectMatchesToFilterCommand : CommandBase
    {
        private readonly InputListParam<TSM.ModelObject> _inModelObjects = new InputListParam<TSM.ModelObject>(ParamInfos.ModelObject);
        private readonly InputParam<string> _inFilterName = new InputParam<string>(ParamInfos.ObjectFilter);


        private readonly OutputListParam<TSM.ModelObject> _outModelObjects = new OutputListParam<TSM.ModelObject>(ParamInfos.FilteredModelObjects);
        private readonly OutputListParam<bool> _outPattern = new OutputListParam<bool>(ParamInfos.FilterPattern);

        internal (List<TSM.ModelObject> ModelObjects, string FilterName) GetInputValues()
        {
            return (_inModelObjects.Value,
                    _inFilterName.Value.Trim());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSM.ModelObject> filteredObjects, List<bool> pattern)
        {
            _outModelObjects.Value = filteredObjects;
            _outPattern.Value = pattern;

            return SetOutput(DA);
        }
    }
}
