using DrawingLink.UI.GH;
using System.Collections.Generic;

namespace DrawingLink.UI.TeklaInteraction
{
    internal class UserInputPicker
    {
        private readonly ModelPicker _modelPicker = new();
        private readonly DrawingPicker _drawingPicker = new();

        public Dictionary<string, TeklaObjects> PickInput(TeklaParams teklaParams)
        {
            CollectTeklaInputs(teklaParams);

            return TransformToDictionary(teklaParams);
        }

        private Dictionary<string, TeklaObjects> TransformToDictionary(TeklaParams teklaParams)
        {
            var result = new Dictionary<string, TeklaObjects>();
            foreach (var param in teklaParams.ModelParams)
                result[param.FieldName] = param.TeklaObjects;

            foreach (var param in teklaParams.DrawingParams)
                result[param.FieldName] = param.TeklaObjects;

            return result;
        }

        private void CollectTeklaInputs(TeklaParams teklaParams)
        {
            foreach (var param in teklaParams.ModelParams)
            {
                if (param.ParamType == ModelParamType.Object)
                {
                    param.Set(_modelPicker.PickObject(param.IsMultiple, param.Prompt));
                }
                else
                {
                    param.Set(_modelPicker.PickPoint(param.ParamType, param.IsMultiple, param.Prompt));
                }
            }
            foreach (var param in teklaParams.DrawingParams)
            {
                if (param.ParamType == DrawingParamType.Object)
                {
                    param.Set(_drawingPicker.PickObject(param.IsMultiple, param.Prompt));
                }
                else if (param.ParamType == DrawingParamType.Line)
                {
                    param.Set(_drawingPicker.PickLine(param.IsMultiple, param.Prompt));
                }
                else
                {
                    param.Set(_drawingPicker.PickPoint(param.IsMultiple, param.Prompt));
                }
            }
        }

        internal bool CanObjectsBePicked(TeklaParams teklaParams, out string warningMessage)
        {
            warningMessage = string.Empty;

            var mode = Tekla.Structures.DrawingInternal.Operation.GetEditMode();

            if (teklaParams.DrawingParams.Count > 0 && mode != Tekla.Structures.DrawingInternal.EditMode.DrawingEditMode)
            {
                warningMessage = "Incorrect Tekla mode. Cannot select drawing objects when in the model.";
                return false;
            }

            return true;
        }
    }
}
