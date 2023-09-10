using Rhino.Geometry;
using GTDrawingLink.Extensions;
using Tekla.Structures.Model.UI;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaView
    {
        public string Name { get; }
        public Plane ViewCoordinateSystem { get; }
        public Plane DisplayCoordinateSystem { get; }
        public Box RestrictionBox { get; }

        public TeklaView(string name, Plane viewCoordinateSystem, Plane displayCoordinateSystem, Box restrictionBox)
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

            var matrixToCurrent= TSG.MatrixFactory.ToCoordinateSystem(view.ViewCoordinateSystem);
            var minPt = matrixToCurrent.Transform(view.WorkArea.MinPoint);
            var maxPt = matrixToCurrent.Transform(view.WorkArea.MaxPoint);
            RestrictionBox = new Box(ViewCoordinateSystem, new BoundingBox(minPt.ToRhino(), maxPt.ToRhino()));
        }
    }
}
