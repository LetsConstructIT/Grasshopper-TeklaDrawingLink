using System;
using System.Collections.Generic;
using Tekla.Structures.Geometry3d;
using TSMUI = Tekla.Structures.Model.UI;

namespace DrawingLink.UI.TeklaInteraction
{
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
}
