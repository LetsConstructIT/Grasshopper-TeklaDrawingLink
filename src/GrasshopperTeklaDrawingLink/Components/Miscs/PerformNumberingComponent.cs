using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;

namespace GTDrawingLink.Components.Miscs
{
    public class PerformNumberingComponent : TeklaComponentBaseNew<PerformNumberingCommand>
    {
        private NumberingMode _mode = NumberingMode.AllModified;

        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.PerformNumbering;

        public PerformNumberingComponent() : base(ComponentInfos.PerformNumberingComponent)
        {
            SetCustomMessage();
        }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var toggle = _command.GetInputValue();
            if (toggle == false)
                return;

            switch (_mode)
            {
                case NumberingMode.AllModified:
                    Tekla.Structures.ModelInternal.Operation.dotStartAction("ModifiedNumbering", "");
                    break;
                case NumberingMode.Selected:
                    if (!ModelInteractor.IsAnythingSelected())
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You have to preselect model objects before running this component. Please use SelectModelObject component.");
                        return;
                    }
                    Tekla.Structures.ModelInternal.Operation.dotStartAction("NumberingForSelectedObjects", "");
                    break;
                default:
                    break;
            }

            _command.SetOutputValue(DA, true);
        }
        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.NumberAllModified.Name, AllModifiedMenuItem_Clicked, true, _mode == NumberingMode.AllModified).ToolTipText = ParamInfos.NumberAllModified.Description;
            Menu_AppendItem(menu, ParamInfos.NumberSelected.Name, SelectedMenuItem_Clicked, true, _mode == NumberingMode.Selected).ToolTipText = ParamInfos.NumberSelected.Description;
            Menu_AppendSeparator(menu);
        }

        private void AllModifiedMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = NumberingMode.AllModified;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SelectedMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = NumberingMode.Selected;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case NumberingMode.AllModified:
                    Message = "All modified";
                    break;
                case NumberingMode.Selected:
                    Message = "Selected";
                    break;
                default:
                    break;
            }
        }
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.NumberAllModified.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.NumberAllModified.Name, ref serializedInt);
            _mode = (NumberingMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        enum NumberingMode
        {
            AllModified,
            Selected
        }
    }

    public class PerformNumberingCommand : CommandBase
    {
        private readonly InputStructParam<bool> _inToggle = new InputStructParam<bool>(ParamInfos.BooleanToggle);

        private readonly OutputParam<bool> _outStatus = new OutputParam<bool>(ParamInfos.NumberingResult);

        internal bool GetInputValue()
        {
            return _inToggle.Value;
        }

        internal Result SetOutputValue(IGH_DataAccess DA, bool status)
        {
            _outStatus.Value = status;

            return SetOutput(DA);
        }
    }
}
