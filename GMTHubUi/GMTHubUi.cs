using GMTHubLib.Utils;
using GMTHubUi.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMTHubUi
{
    public partial class GMTHubUi : Form
    {
        public GMTHubUi()
        {
            InitializeComponent();

            // Set console output
            ConsoleLog.textBox = this.richTextBoxTelemetry;

            // Hack pour key listener: si le jeu démarre l'event est apparement perdu, d'où le timer
            GlobalKeyboardHook gkh = new GlobalKeyboardHook();
            gkh.HookedKeys.Add(System.Windows.Forms.Keys.I);
            System.Timers.Timer t = new System.Timers.Timer(5000);
            t.Elapsed += (sender, args) => {
                gkh.KeyDown -= KListener_KeyDown;
                gkh.KeyDown += KListener_KeyDown;
            };
            t.AutoReset = true;
            t.Enabled = true;

            Task.Factory.StartNew(() =>
            {
                GMTHubLib.GMTHubLib.Start();
            });
        }

        void KListener_KeyDown(object sender, KeyEventArgs e)
        {
            GMTConfig.NextPage();
            ConsoleLog.Debug("Key event [Next page]: " + e.KeyCode.ToString());
        }
    }
}
