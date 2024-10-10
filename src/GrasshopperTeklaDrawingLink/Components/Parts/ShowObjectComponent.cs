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
    public class ShowObjectComponent : TeklaComponentBase
    {
        private OperationMode _mode = OperationMode.View;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Properties.Resources.ShowObject;

        public ShowObjectComponent() : base(ComponentInfos.ShowObjectComponent)
        {
            SetCustomMessage();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingPartParam, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ComponentInfos.DrawingPartParam, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            object input = null;
            DA.GetData(ComponentInfos.DrawingPartParam.Name, ref input);

            if (input is TeklaDatabaseObjectGoo databaseGoo && databaseGoo.Value is ModelObject modelObject)
            {
                if (_mode == OperationMode.View)
                    modelObject.Hideable.ShowInDrawingView();
                else
                    modelObject.Hideable.ShowInDrawing();

                modelObject.Modify();
            }

            DA.SetData(ComponentInfos.DrawingPartParam.Name, input);
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
            switch (_mode)
            {
                case OperationMode.View:
                    Message = "View scope";
                    break;
                case OperationMode.Drawing:
                    Message = "Drawing scope";
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
