using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrawingLink.UI
{
    /// <summary>
    /// Interaction logic for MessageBoxWindow.xaml
    /// </summary>
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow()
        {
            InitializeComponent();
        }

        public void AddMessages(IEnumerable<string> messages)
        {
            if (messages == null)
                return;

            var textBox = this.textBox;

            textBox.Text = string.IsNullOrWhiteSpace(textBox.Text) ? "" : (textBox.Text + Environment.NewLine);
            textBox.Text += string.Join(Environment.NewLine,
                from m in messages
                select m.Trim(new char[]
                {
                    '\n',
                    '\r',
                    '\t',
                    ' '
                }).Replace("\r\n", "\n").Replace("\n", Environment.NewLine));
        }
    }
}
