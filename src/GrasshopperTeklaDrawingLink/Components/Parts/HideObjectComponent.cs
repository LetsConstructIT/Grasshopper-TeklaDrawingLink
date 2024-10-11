using GH_IO.Serialization;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using System.Windows.Forms;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Parts
{
    public class HideObjectComponent : TeklaComponentBase
    {
        private OperationMode _mode = OperationMode.View;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.HideObject;

        public HideObjectComponent() : base(ComponentInfos.HideObjectComponent)
        {
            SetCustomMessage();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingObjectParam, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object input = null;
            DA.GetData(ComponentInfos.DrawingObjectParam.Name, ref input);

            if (input is TeklaDatabaseObjectGoo databaseGoo && databaseGoo.Value is ModelObject modelObject)
            {
                if (_mode == OperationMode.View)
                    modelObject.Hideable.HideFromDrawingView();
                else
                    modelObject.Hideable.HideFromDrawing();

                modelObject.Modify();
            }

            DA.SetData(ComponentInfos.DrawingObjectParam.Name, input);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.ViewOperation.Name, ViewMenuItem_Clicked, true, _mode == OperationMode.View).ToolTipText = ParamInfos.ViewOperation.Description;
            Menu_AppendItem(menu, ParamInfos.DrawingOperation.Name, DrawingMenuItem_Clicked, true, _mode == OperationMode.Drawing).ToolTipText = ParamInfos.DrawingOperation.Description;
            Menu_AppendSeparator(menu);
        }

        private void ViewMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = OperationMode.View;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void DrawingMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = OperationMode.Drawing;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            Message = _mode switch
            {
                OperationMode.View => "View scope",
                OperationMode.Drawing => "Drawing scope",
                _ => "",
            };
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
            _mode = (OperationMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        enum OperationMode
        {
            View,
            Drawing
        }
    }
}
