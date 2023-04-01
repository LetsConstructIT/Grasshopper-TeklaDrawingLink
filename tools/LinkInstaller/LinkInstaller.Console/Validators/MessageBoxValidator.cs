using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkInstaller.Validators
{
    public abstract class MessageBoxValidator
    {
        public void ShowErrorToUser(params string[] messages)
        {
            var mergedMessage = string.Join(Environment.NewLine, messages);
            System.Windows.Forms.MessageBox.Show(mergedMessage, "Error");
        }

        public abstract bool IsValid();
    }
}
