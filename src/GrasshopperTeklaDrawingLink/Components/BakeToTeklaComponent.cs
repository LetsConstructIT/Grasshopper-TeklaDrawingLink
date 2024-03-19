using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Components
{
    public class BakeToTeklaComponent : TeklaComponentBaseNew<BakeToTeklaCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.BakeToTekla;
        public BakeToTeklaComponent() : base(ComponentInfos.BakeToTeklaComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (IGH_Structure component, bool trigger) = _command.GetInputValues();
            _command.SetOutputValues(DA, component);
            if (!trigger)
                return;

            var input = Params.Input[Params.IndexOfInputParam(ParamInfos.TeklaComponent.Name)];
            foreach (var source in input.Sources)
            {
                var docObject = source.Attributes.GetTopLevel.DocObject;
                docObject
                    .GetType()
                    .GetMethod("BakeToTekla")?
                    .Invoke(docObject, null);
            }
        }
    }

    public class BakeToTeklaCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_Goo> _inComponent = new InputTreeParam<IGH_Goo>(ParamInfos.TeklaComponent);
        private readonly InputStructParam<bool> _inTrigger = new InputStructParam<bool>(ParamInfos.BooleanToogle);

        private readonly OutputTreeParam<IGH_Goo> _outComponent = new OutputTreeParam<IGH_Goo>(ParamInfos.TeklaComponent, 0);

        internal (IGH_Structure Component, bool Trigger) GetInputValues()
        {
            return (_inComponent.Tree,
                    _inTrigger.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure component)
        {
            _outComponent.Value = component;

            return SetOutput(DA);
        }
    }
}
