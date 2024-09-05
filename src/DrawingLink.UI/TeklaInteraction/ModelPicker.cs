using DrawingLink.UI.GH;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;
using TSDUI = Tekla.Structures.Drawing.UI;

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
            {
                result[param.FieldName] = param.TeklaObjects;
            }

            foreach (var param in teklaParams.DrawingParams)
            {
                result[param.FieldName] = param.TeklaObjects;
            }

            return result;
        }

        private void CollectTeklaInputs(TeklaParams teklaParams)
        {
            // check in which Tekla area we are
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

    internal class ModelPicker
    {
        private readonly TSMUI.Picker _picker = new();

        public List<Tekla.Structures.Model.ModelObject> PickObject(bool multiple, string prompt)
        {
            var result = new List<Tekla.Structures.Model.ModelObject>();
            if (multiple)
            {
                var modelObjectEnumerator = _picker.PickObjects(TSMUI.Picker.PickObjectsEnum.PICK_N_OBJECTS, prompt);
                foreach (Tekla.Structures.Model.ModelObject item in modelObjectEnumerator)
                    result.Add(item);
            }
            else
            {
                var modelObject = _picker.PickObject(TSMUI.Picker.PickObjectEnum.PICK_ONE_OBJECT, prompt);
                result.Add(modelObject);
            }

            return result;
        }

        internal List<Point> PickPoint(ModelParamType paramType, bool isMultiple, string prompt)
        {
            var result = new List<Point>();
            var arrayList = _picker.PickPoints(GetTeklaType(paramType, isMultiple), prompt);
            foreach (Point point in arrayList)
                result.Add(point);

            return result;
        }

        private TSMUI.Picker.PickPointEnum GetTeklaType(ModelParamType paramType, bool isMultiple)
        {
            return paramType switch
            {
                ModelParamType.Point => isMultiple ? TSMUI.Picker.PickPointEnum.PICK_POLYGON : TSMUI.Picker.PickPointEnum.PICK_ONE_POINT,
                ModelParamType.Line => TSMUI.Picker.PickPointEnum.PICK_TWO_POINTS,
                ModelParamType.Polyline => TSMUI.Picker.PickPointEnum.PICK_POLYGON,
                ModelParamType.Face => TSMUI.Picker.PickPointEnum.PICK_FACE,
                _ => throw new ArgumentOutOfRangeException(nameof(paramType)),
            };
        }
    }

    internal class DrawingPicker
    {
        internal List<DatabaseObject> PickObject(bool isMultiple, string prompt)
        {
            var picker = new DrawingHandler().GetPicker();
            var drawingObjects = new List<DatabaseObject>();

            if (isMultiple)
            {
                try
                {
                    while (true)
                    {
                        var result = picker.PickObject(prompt);
                        drawingObjects.Add(result.Item1);
                    }
                }
                catch (PickerInterruptedException)
                {
                    return drawingObjects;
                }
            }
            else
            {
                var result = picker.PickObject(prompt);
                drawingObjects.Add(result.Item1);
            }

            return drawingObjects;
        }


        internal List<Point> PickPoint(bool isMultiple, string prompt)
        {
            var picker = new DrawingHandler().GetPicker();
            var points = new List<Point>();
            if (isMultiple)
            {
                try
                {
                    while (true)
                    {
                        var result = picker.PickPoint("Pick point");
                        points.Add(result.Item1);
                    }
                }
                catch (PickerInterruptedException)
                {
                    return points;
                }
            }
            {
                var result = picker.PickPoint("Pick point");
                points.Add(result.Item1);
            }

            return points;
        }
    }
}
