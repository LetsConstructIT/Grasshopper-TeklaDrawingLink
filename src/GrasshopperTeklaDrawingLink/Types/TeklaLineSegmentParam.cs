using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Types
{
    public class TeklaLineSegmentParam : GH_PersistentParam<TeklaLineSegmentGoo>, IGH_PreviewObject
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DrawingLine;

        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        public bool Hidden { get; set; }

        public bool IsPreviewCapable => true;

        public BoundingBox ClippingBox => default;

        public TeklaLineSegmentParam() : base(ComponentInfos.TeklaLineSegmentParam)
        {
        }

        protected override GH_GetterResult Prompt_Singular(ref TeklaLineSegmentGoo value)
        {
            var line = DrawingInteractor.PickLine();

            if (line == null)
                return GH_GetterResult.cancel;

            value = new TeklaLineSegmentGoo(line);
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Plural(ref List<TeklaLineSegmentGoo> values)
        {
            var lines = DrawingInteractor.PickLines();

            if (lines == null)
                return GH_GetterResult.cancel;

            values = new List<TeklaLineSegmentGoo>();
            foreach (var line in lines)
                values.Add(new TeklaLineSegmentGoo(line));

            return GH_GetterResult.success;
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (this.Locked || Hidden)
                return;

            foreach (var line in this.VolatileData.AllData(true).Select(d => ((GH_Goo<TSG.LineSegment>)d).Value))
            {
                if (line != null)
                    args.Display.DrawLine(line.Point1.ToRhino(), line.Point2.ToRhino(), this.Attributes.Selected ? args.WireColour_Selected : args.WireColour, 1);
            }
        }

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
        }
        public override bool Read(GH_IReader reader)
        {
            bool result = base.Read(reader);
            base.PersistentData.Clear();

            var list = new List<TSG.LineSegment>();
            string value = null;
            int num = 0;

            while (reader.TryGetString("TeklaLineSegments", num++, ref value))
                list.Add(ParseLineString(value));

            SetPersistentData(list.Select((TSG.LineSegment l) => new TeklaLineSegmentGoo(l)));
            return result;
        }

        public override bool Write(GH_IWriter writer)
        {
            bool result = base.Write(writer);
            if (SourceCount > 0)
                return result;

            var list = (from d in base.VolatileData.AllData(skipNulls: true)
                        select ((GH_Goo<TSG.LineSegment>)d).Value).ToList();

            for (int i = 0; i < list.Count; i++)
                writer.SetString("TeklaLineSegments", i, GetLineString(list[i]));

            return result;
        }

        private string GetLineString(TSG.LineSegment line)
        {
            var p1 = $"{line.Point1.X.ToString(CultureInfo.InvariantCulture)};{line.Point1.Y.ToString(CultureInfo.InvariantCulture)};{line.Point1.Z.ToString(CultureInfo.InvariantCulture)}";
            var p2 = $"{line.Point2.X.ToString(CultureInfo.InvariantCulture)};{line.Point2.Y.ToString(CultureInfo.InvariantCulture)};{line.Point2.Z.ToString(CultureInfo.InvariantCulture)}";
            return $"{p1};{p2}";
        }

        private TSG.LineSegment ParseLineString(string pointString)
        {
            if (string.IsNullOrEmpty(pointString))
                return null;

            var array = pointString.Split(';');
            if (array.Length != 6)
                return null;

            var p1 = new TSG.Point(double.Parse(array[0], CultureInfo.InvariantCulture),
                                   double.Parse(array[1], CultureInfo.InvariantCulture),
                                   double.Parse(array[2], CultureInfo.InvariantCulture));
            var p2 = new TSG.Point(double.Parse(array[3], CultureInfo.InvariantCulture),
                                   double.Parse(array[4], CultureInfo.InvariantCulture),
                                   double.Parse(array[5], CultureInfo.InvariantCulture));

            return new TSG.LineSegment(p1, p2);
        }
    }
}
