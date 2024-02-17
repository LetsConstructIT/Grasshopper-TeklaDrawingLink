using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures.Dialog;

namespace TeklaPlugins
{
    public partial class GHDrawingForm2 : PluginFormBase
    {
        public GHDrawingForm2()
        {
            InitializeComponent();
        }

        private void okCancel1_OkClicked(object sender, EventArgs e)
        {
            this.Modify();
        }

        private void okCancel1_CancelClicked(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
