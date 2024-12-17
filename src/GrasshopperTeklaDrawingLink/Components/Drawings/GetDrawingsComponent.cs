using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class GetDrawingsComponent : TeklaComponentBaseNew<GetDrawingsCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override Bitmap Icon => Properties.Resources.AllDrawings;

        public GetDrawingsComponent() : base(ComponentInfos.GetDrawingsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var trigger = _command.GetInputValues();
            if (!trigger)
                return;

            var drawings = GetDrawings();

            _command.SetOutputValues(DA, drawings);
        }

        private IEnumerable<Drawing> GetDrawings()
        {
            var drawingEnumerator = DrawingInteractor.DrawingHandler.GetDrawings();
            drawingEnumerator.SelectInstances = false;
            while (drawingEnumerator.MoveNext())
            {
                yield return drawingEnumerator.Current;
            }
        }
    }

    public class GetDrawingsCommand : CommandBase
    {
        private readonly InputStructParam<bool> _inToggle = new InputStructParam<bool>(ParamInfos.BooleanToggle);
        private readonly OutputListParam<Drawing> _outDrawings = new OutputListParam<Drawing>(ParamInfos.Drawing);

        internal bool GetInputValues()
        {
            return _inToggle.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IEnumerable<Drawing> drawings)
        {
            _outDrawings.Value = drawings;

            return SetOutput(DA);
        }
    }
}
