using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public interface IBakeable
    {
        void BakeToTekla();
        void DeleteObjects();
        List<DatabaseObject> GetObjects();
    }
}