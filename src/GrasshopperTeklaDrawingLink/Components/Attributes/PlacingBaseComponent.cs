using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class PlacingBaseComponent : TeklaComponentBaseNew<PlacingBaseCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Resources.PlacingBase;

        public PlacingBaseComponent() : base(ComponentInfos.PlacingBaseComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (type, point) = _command.GetInputValues();

            var placingType = PlacingTypes.PointPlacing();
            if (type == LeaderLineType.LeaderLine)
            {
                placingType = PlacingTypes.LeaderLinePlacing(point.ToTekla());
            }

            _command.SetOutputValues(DA, placingType);
        }
    }

    public class PlacingBaseCommand : CommandBase
    {
        private readonly InputOptionalStructParam<LeaderLineType> _inLeaderLine = new InputOptionalStructParam<LeaderLineType>(ParamInfos.LeaderLineType, LeaderLineType.NoLeaderLine);
        private readonly InputOptionalParam<GH_Point> _inPoint = new InputOptionalParam<GH_Point>(ParamInfos.LeaderLineStartingPoint, new GH_Point(new Point3d()));

        private readonly OutputParam<PlacingBase> _outPlacing = new OutputParam<PlacingBase>(ParamInfos.PlacingType);

        internal (LeaderLineType leaderLine, Point3d point) GetInputValues()
        {
            return (
                _inLeaderLine.Value,
                _inPoint.Value.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, PlacingBase placing)
        {
            _outPlacing.Value = placing;

            return SetOutput(DA);
        }
    }

    public enum LeaderLineType
    {
        NoLeaderLine,
        LeaderLine
    }
}
