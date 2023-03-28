using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;
using T3D = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingPointParam : GH_PersistentParam<TeklaPointGoo>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DrawingPoint;

        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public TeklaDrawingPointParam() : base(ComponentInfos.TeklaDrawingPointParam)
        {
		}

		protected override GH_GetterResult Prompt_Singular(ref TeklaPointGoo value)
		{
			var point = DrawingInteractor.PickPoint();

			if (point == null)
				return GH_GetterResult.cancel;

			value = new TeklaPointGoo(point);
			return GH_GetterResult.success;
		}

		protected override GH_GetterResult Prompt_Plural(ref List<TeklaPointGoo> values)
        {
            var points = DrawingInteractor.PickPoints();

            if (points == null)
                return GH_GetterResult.cancel;

            values = new List<TeklaPointGoo>();
            foreach (var point in points)
                values.Add(new TeklaPointGoo(point));

            return GH_GetterResult.success;
        }
	}
}
