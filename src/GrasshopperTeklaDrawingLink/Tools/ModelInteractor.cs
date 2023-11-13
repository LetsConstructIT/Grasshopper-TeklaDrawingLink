using GTDrawingLink.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using Tekla.Structures;
using Tekla.Structures.Geometry3d;
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

        internal static bool IsAnythingSelected()
        {
            return _modelObjectSelector.GetSelectedObjects().GetSize() > 0;
        }

        internal static bool IsConnected()
        {
            return Model.GetConnectionStatus();
        }

        internal static AABB? GetAabb(Identifier modelIdentifier)
        {
            var modelObject = GetModelObject(modelIdentifier);
            if (modelObject is BoltGroup bolt)
            {
                return bolt.GetSolid().ToAabb();
            }
            else if (modelObject is Part part)
            {
                return part.GetSolid().ToAabb();
            }
            else if (modelObject is Reinforcement rebar)
            {
                return rebar.GetSolid().ToAabb();
            }

            return null;
        }
    }
}
