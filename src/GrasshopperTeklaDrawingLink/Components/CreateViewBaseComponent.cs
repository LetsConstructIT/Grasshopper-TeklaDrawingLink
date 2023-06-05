using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Windows.Forms;

namespace GTDrawingLink.Components
{
    public abstract class CreateViewBaseComponent : TeklaComponentBase
    {
        protected bool _loadAttributesWithMacro;

        protected CreateViewBaseComponent(GH_InstanceDescription info) : base(info)
        {
            SetCustomMessage();
        }

        protected void LoadAttributesWithMacroIfNecessary(Tekla.Structures.Drawing.View view, string attributesName)
        {
            if (!_loadAttributesWithMacro ||
                string.IsNullOrEmpty(attributesName))
                return;

            DrawingInteractor.Highlight(view);

            var macroContent = Macros.LoadViewProperties(attributesName);
            var macroPath = new LightweightMacroBuilder()
                        .SaveMacroAndReturnRelativePath(macroContent);

            Tekla.Structures.Model.Operations.Operation.RunMacro(macroPath);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Don't use macro", DoNotUseMacroMenuItem_Clicked, true, !_loadAttributesWithMacro).ToolTipText = "The view settings could be not be fully loaded (detailed settings/filters)";
            Menu_AppendItem(menu, "Use macro", UseMacroMenuItem_Clicked, true, _loadAttributesWithMacro).ToolTipText = "The view settings will be loaded with the macro - it will take longer";
            Menu_AppendSeparator(menu);
        }

        private void SetCustomMessage()
        {
            if (_loadAttributesWithMacro)
                base.Message = "use macro for attributes";
            else
                base.Message = "";
        }

        private void DoNotUseMacroMenuItem_Clicked(object sender, EventArgs e)
        {
            _loadAttributesWithMacro = false;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void UseMacroMenuItem_Clicked(object sender, EventArgs e)
        {
            _loadAttributesWithMacro = true;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean(ParamInfos.ViewAttributesLoadedByMacro.Name, _loadAttributesWithMacro);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            reader.TryGetBoolean(ParamInfos.ViewAttributesLoadedByMacro.Name, ref _loadAttributesWithMacro);
            SetCustomMessage();
            return base.Read(reader);
        }
    }
}
