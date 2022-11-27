using GMTHubLib.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMTHubUi.Classes
{
    public class ConsoleTextBox : System.Windows.Forms.RichTextBox, IConsoleTextBox
    {
        public void AppendText(string text, string color)
        {
            Color oColor = Color.FromName(color);
            if (InvokeRequired)
            {
                Action safeWrite = delegate {
                    SelectionStart = TextLength;
                    SelectionLength = 0;
                    SelectionColor = oColor;
                    AppendText(text + Environment.NewLine);
                    SelectionColor = ForeColor;
                    ScrollToCaret();
                };
                Invoke(safeWrite);
            }
            else
            {
                SelectionStart = TextLength;
                SelectionLength = 0;
                SelectionColor = oColor;
                AppendText(text + Environment.NewLine);
                SelectionColor = ForeColor;
                ScrollToCaret();
            }
        }
    }
}
