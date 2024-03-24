using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Plugins
{
    public class PickerInputComponent : TeklaComponentBase, IGH_VariableParameterComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.PickerInput;

        public PickerInputComponent() : base(ComponentInfos.PickerInputComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.PickerInputInput, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.PickerInput, GH_ParamAccess.item);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Input;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return side == GH_ParameterSide.Input &&
                Params.Input.Count > 1;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            Param_GenericObject result = new Param_GenericObject
            {
                Name = ParamInfos.PickerInputInput.Name,
                NickName = ParamInfos.PickerInputInput.NickName,
                Description = ParamInfos.PickerInputInput.Description,
                Access = GH_ParamAccess.list
            };

            ExpireSolution(recompute: true);
            return result;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            ExpireSolution(recompute: true);
            return true;
        }

        public void VariableParameterMaintenance()
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            PickerInputGoo[][] pickerInputInputs = new PickerInputGoo[Params.Input.Count][];
            for (int j = 0; j < Params.Input.Count; j++)
            {
                List<PickerInputGoo> pickerInput = new List<PickerInputGoo>();
                if (!DA.GetDataList(j, pickerInput))
                    return;

                pickerInputInputs[j] = pickerInput.ToArray();
            }

            PluginPickerInput pluginPickerInput = new PluginPickerInput();
            foreach (var pickerInputInput in pickerInputInputs)
                foreach (var pickerInput in pickerInputInput)
                    pluginPickerInput.Add(pickerInput.Value);

            DA.SetData(ParamInfos.PickerInput.Name, pluginPickerInput);
        }
    }
}