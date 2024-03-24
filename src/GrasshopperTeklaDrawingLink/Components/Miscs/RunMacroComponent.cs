using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GTDrawingLink.Components.Miscs
{
    public class RunMacroComponent : TeklaComponentBase
    {
        private MacroMode _mode = MacroMode.Model;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.RunMacro;

        public RunMacroComponent() : base(ComponentInfos.RunMacroComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.ModelMacro.Name, ModelMacroMenuItem_Clicked, true, _mode == MacroMode.Model).ToolTipText = ParamInfos.ModelMacro.Description;
            Menu_AppendItem(menu, ParamInfos.DrawingMacro.Name, DrawingMacroMenuItem_Clicked, true, _mode == MacroMode.Drawing).ToolTipText = ParamInfos.DrawingMacro.Description;
            Menu_AppendItem(menu, ParamInfos.DynamicMacro.Name, DynamicMacroMenuItem_Clicked, true, _mode == MacroMode.Dynamic).ToolTipText = ParamInfos.DynamicMacro.Description;
            Menu_AppendSeparator(menu);
        }

        private void ModelMacroMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = MacroMode.Model;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void DrawingMacroMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = MacroMode.Drawing;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void DynamicMacroMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = MacroMode.Dynamic;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case MacroMode.Model:
                    Message = "'modeling' directory";
                    break;
                case MacroMode.Drawing:
                    Message = "'drawing' directory";
                    break;
                case MacroMode.Dynamic:
                    Message = "on-fly macro";
                    break;
                default:
                    Message = "";
                    break;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.ModelMacro.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.ModelMacro.Name, ref serializedInt);
            _mode = (MacroMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Trigger to run macro", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Macro", "M", "Macro name or content (for dynamic mode)", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Result", "R", "True when macro is completed", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var run = false;
            DA.GetData("Run", ref run);
            if (!run)
                return;

            var macro = "";
            DA.GetData("Macro", ref macro);
            if (string.IsNullOrEmpty(macro))
                return;

            var macroPath = macro;
            switch (_mode)
            {
                case MacroMode.Model:
                    macroPath = GetPathWithExtension(macroPath);
                    break;
                case MacroMode.Drawing:
                    macroPath = $@"..\drawings\{GetPathWithExtension(macroPath)}";
                    break;
                case MacroMode.Dynamic:
                    var macroContent = macroPath;
                    macroPath = new LightweightMacroBuilder()
                        .SaveMacroAndReturnRelativePath(macroContent);
                    break;
                default:
                    break;
            }

            var status = Tekla.Structures.Model.Operations.Operation.RunMacro(macroPath);
            DA.SetData("Result", status);
        }


        private string GetPathWithExtension(string path)
        {
            if (!path.EndsWith(".cs"))
                return path + ".cs";
            else
                return path;
        }

        enum MacroMode
        {
            Model,
            Drawing,
            Dynamic
        }
    }
}
