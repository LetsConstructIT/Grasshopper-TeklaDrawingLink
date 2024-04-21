using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingPointParam : GH_PersistentParam<TeklaPointGoo>, IGH_PreviewObject
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DrawingPoint;

        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public bool Hidden { get; set; }

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => default;

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

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (this.Locked || Hidden)
                return;

            foreach (var point in this.VolatileData.AllData(true).Select(d => ((GH_Goo<TSG.Point>)d).Value))
            {
                if (point != null)
                    args.Display.DrawPoint(point.ToRhino(), PointStyle.X, 5, this.Attributes.Selected ? args.WireColour_Selected : args.WireColour);
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }
    }
}
