using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Views
{
    public class MoveViewToPointComponent : TeklaComponentBaseNew<MoveViewToPointCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.MoveViewToPoint;
        public MoveViewToPointComponent() : base(ComponentInfos.MoveViewToPointComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (view, point, movementType) = _command.GetInputValues();
            if ((int)movementType == -1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Unknown view movement type. Provide one of the allowed values.");
                return;
            }

            MoveView(view, point.ToTekla(), movementType);

            _command.SetOutputValues(DA, view);
        }

        private void MoveView(View view, TSG.Point destination, ViewMovementType movementType)
        {
            view.Select();
            var viewBox = view.GetAxisAlignedBoundingBox();
            var moveVector = new TSG.Vector();
            switch (movementType)
            {
                case ViewMovementType.LowerLeft:
                    moveVector = new TSG.Vector(destination - viewBox.LowerLeft);
                    break;
                case ViewMovementType.UpperLeft:
                    moveVector = new TSG.Vector(destination - viewBox.UpperLeft);
                    break;
                case ViewMovementType.UpperRight:
                    moveVector = new TSG.Vector(destination - viewBox.UpperRight);
                    break;
                case ViewMovementType.LowerRight:
                    moveVector = new TSG.Vector(destination - viewBox.LowerRight);
                    break;
                case ViewMovementType.OriginHorizontally:
                    var dy = destination.Y - view.Origin.Y;
                    moveVector = new TSG.Vector(0, dy, 0);
                    break;
                case ViewMovementType.OriginVertically:
                    var dx = destination.X - view.Origin.X;
                    moveVector = new TSG.Vector(dx, 0, 0);
                    break;
                default:
                    break;
            }
            view.Origin += moveVector;
            view.Modify();
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeComponent).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }
    }

    public class MoveViewToPointCommand : CommandBase
    {
        private readonly InputParam<View> _inView = new InputParam<View>(ParamInfos.View);
        private readonly InputPoint _inDestination = new InputPoint(ParamInfos.DestinationPoint);
        private readonly InputStructParam<ViewMovementType> _inMovementType = new InputStructParam<ViewMovementType>(ParamInfos.ViewMovementType);

        private readonly OutputParam<View> _outView = new OutputParam<View>(ParamInfos.View);

        internal (View view, Point3d point, ViewMovementType movementType) GetInputValues()
        {
            return (_inView.Value, _inDestination.Value, _inMovementType.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, View view)
        {
            _outView.Value = view;

            return SetOutput(DA);
        }
    }

    public enum ViewMovementType
    {
        LowerLeft = 0,
        UpperLeft = 1,
        UpperRight = 2,
        LowerRight = 3,
        OriginHorizontally = 4,
        OriginVertically = 5
    }
}
