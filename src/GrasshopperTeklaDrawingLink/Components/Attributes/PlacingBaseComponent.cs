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
            var (type, startPoint, endPoint) = _command.GetInputValues();

            var placingType = PlacingTypes.PointPlacing();
            switch (type)
            {
                case LeaderLineType.LeaderLine:
                    placingType = PlacingTypes.LeaderLinePlacing(startPoint.ToTekla());
                    break;
                case LeaderLineType.AlongLine:
                    placingType = PlacingTypes.AlongLinePlacing(startPoint.ToTekla(), endPoint.ToTekla());
                    break;
                case LeaderLineType.BaseLine:
                    placingType = PlacingTypes.BaseLinePlacing(startPoint.ToTekla(), endPoint.ToTekla());
                    break;
                default:
                    break;
            }

            _command.SetOutputValues(DA, placingType);
        }
    }

    public class PlacingBaseCommand : CommandBase
    {
        private readonly InputOptionalStructParam<LeaderLineType> _inLeaderLine = new InputOptionalStructParam<LeaderLineType>(ParamInfos.LeaderLineType, LeaderLineType.NoLeaderLine);
        private readonly InputOptionalParam<GH_Point> _inStartPoint = new InputOptionalParam<GH_Point>(ParamInfos.LeaderLineStartPoint, new GH_Point(new Point3d()));
        private readonly InputOptionalParam<GH_Point> _inEndPoint = new InputOptionalParam<GH_Point>(ParamInfos.LeaderLineEndPoint, new GH_Point(new Point3d()));

        private readonly OutputParam<PlacingBase> _outPlacing = new OutputParam<PlacingBase>(ParamInfos.PlacingType);

        internal (LeaderLineType leaderLine, Point3d startPoint, Point3d endPoint) GetInputValues()
        {
            return (
                _inLeaderLine.Value,
                _inStartPoint.Value.Value,
                _inEndPoint.Value.Value);
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
        LeaderLine,
        AlongLine,
        BaseLine
    }
}
