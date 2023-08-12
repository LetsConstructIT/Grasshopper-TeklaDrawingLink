using Rhino.Geometry;
using GTDrawingLink.Extensions;
using Tekla.Structures.Model.UI;

namespace GTDrawingLink.Types
{
    public class TeklaView
    {
        public string Name { get; }
        public Plane ViewCoordinateSystem { get; }
        public Plane DisplayCoordinateSystem { get; }
        public BoundingBox RestrictionBox { get; }

        public TeklaView(string name, Plane viewCoordinateSystem, Plane displayCoordinateSystem, BoundingBox restrictionBox)
        {
            Name = name ?? "";
            ViewCoordinateSystem = viewCoordinateSystem;
            DisplayCoordinateSystem = displayCoordinateSystem;
            RestrictionBox = restrictionBox;
        }

        public TeklaView(View view)
        {
            Name = view.Name;
            ViewCoordinateSystem = view.ViewCoordinateSystem.ToRhino();
            DisplayCoordinateSystem = view.DisplayCoordinateSystem.ToRhino();
            RestrictionBox = view.WorkArea.ToRhino();
        }
    }
}
