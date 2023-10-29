using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;

namespace GTDrawingLink.Types
{
    internal class ModelInteractor
    {
        internal static Model Model { get; private set; }

        private static TSMUI.ModelObjectSelector _modelObjectSelector;

        static ModelInteractor()
        {
            Model = new Model();

            _modelObjectSelector = new TSMUI.ModelObjectSelector();
        }

        internal static ModelObject GetModelObject(Identifier identifier)
        {
            return Model.SelectModelObject(identifier);
        }

        internal static void SelectModelObjects(List<ModelObject> modelObjects)
        {
            var arrayList = new ArrayList();
            foreach (var item in modelObjects)
                arrayList.Add(item);

            _modelObjectSelector.Select(arrayList);
        }

        internal static bool IsConnected()
        {
            return Model.GetConnectionStatus();
        }
    }
}
