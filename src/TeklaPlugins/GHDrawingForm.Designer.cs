namespace TeklaPlugins
{
    partial class GHDrawingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okCancel1 = new Tekla.Structures.Dialog.UIControls.OkCancel();
            this.saveLoad1 = new Tekla.Structures.Dialog.UIControls.SaveLoad();
            this.SuspendLayout();
            // 
            // okCancel1
            // 
            this.structuresExtender.SetAttributeName(this.okCancel1, null);
            this.structuresExtender.SetAttributeTypeName(this.okCancel1, null);
            this.structuresExtender.SetBindPropertyName(this.okCancel1, null);
            this.okCancel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.okCancel1.Location = new System.Drawing.Point(0, 192);
            this.okCancel1.Name = "okCancel1";
            this.okCancel1.Size = new System.Drawing.Size(581, 30);
            this.okCancel1.TabIndex = 0;
            this.okCancel1.OkClicked += new System.EventHandler(this.okCancel1_OkClicked);
            this.okCancel1.CancelClicked += new System.EventHandler(this.okCancel1_CancelClicked);
            // 
            // saveLoad1
            // 
            this.structuresExtender.SetAttributeName(this.saveLoad1, null);
            this.structuresExtender.SetAttributeTypeName(this.saveLoad1, null);
            this.saveLoad1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.structuresExtender.SetBindPropertyName(this.saveLoad1, null);
            this.saveLoad1.Dock = System.Windows.Forms.DockStyle.Top;
            this.saveLoad1.HelpFileType = Tekla.Structures.Dialog.UIControls.SaveLoad.HelpFileTypeEnum.General;
            this.saveLoad1.HelpKeyword = "";
            this.saveLoad1.HelpUrl = "";
            this.saveLoad1.Location = new System.Drawing.Point(0, 0);
            this.saveLoad1.Name = "saveLoad1";
            this.saveLoad1.SaveAsText = "";
            this.saveLoad1.Size = new System.Drawing.Size(581, 43);
            this.saveLoad1.TabIndex = 1;
            this.saveLoad1.UserDefinedHelpFilePath = null;
            // 
            // GHDrawingForm
            // 
            this.structuresExtender.SetAttributeName(this, null);
            this.structuresExtender.SetAttributeTypeName(this, null);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.structuresExtender.SetBindPropertyName(this, null);
            this.ClientSize = new System.Drawing.Size(581, 222);
            this.Controls.Add(this.saveLoad1);
            this.Controls.Add(this.okCancel1);
            this.Name = "GHDrawingForm";
            this.Text = "GHDrawingForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Tekla.Structures.Dialog.UIControls.OkCancel okCancel1;
        private Tekla.Structures.Dialog.UIControls.SaveLoad saveLoad1;
    }
}