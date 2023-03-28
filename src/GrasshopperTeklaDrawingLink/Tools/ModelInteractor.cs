using Tekla.Structures;
using Tekla.Structures.Model;

namespace GTDrawingLink.Types
{
    internal class ModelInteractor
    {
        internal static Model Model { get; private set; }

        static ModelInteractor()
        {
            Model = new Model();
        }

        internal static ModelObject GetModelObject(Identifier identifier)
        {
            return Model.SelectModelObject(identifier);
        }

        internal static bool IsConnected()
        {
            return Model.GetConnectionStatus();
        }
    }
}
