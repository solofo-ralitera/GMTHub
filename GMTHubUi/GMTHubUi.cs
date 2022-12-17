using GMTHubLib.Utils;
using GMTHubUi.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using SharpDX.DirectInput;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;

namespace GMTHubUi
{
    public partial class GMTHubUi : Form
    {
        protected List<System.Timers.Timer> timerKeyListeners = new List<System.Timers.Timer>();
        protected DirectInput DirectInput = new DirectInput();
        protected List<Joystick> Joysticks = new List<Joystick>();
        protected System.Timers.Timer JoyTimer = new System.Timers.Timer(250);
        HashSet<string> JoyPageKeys = new HashSet<string>();

        public GMTHubUi()
        {
            InitializeComponent();
        }

        private void GMTHubUi_Load(object sender, EventArgs e)
        {
            // Set console output
            ConsoleLog.textBox = this.richTextBoxTelemetry;
        }

        private void GMTHubUi_Shown(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                GMTHubLib.GMTHubLib.Start();
            });
            // Listen to joystick and keyboard (used to paginate lcd)
            SetKeyboardListener();
            SetGameControlListener();
        }

        void RemoveKeyTimers()
        {
            timerKeyListeners.ForEach((System.Timers.Timer t) =>
            {
                t.Dispose();
            });
        }

        void SetKeyboardListener()
        {
            RemoveKeyTimers();
            while (!GMTConfig.IsLoaded)
            {
            }
            JoyPageKeys.Clear();
            foreach (byte boardNumber in GMTConfig.Boards.Keys)
            {
                GMTConfig.Boards.TryGetValue(boardNumber, out BoardConfig boardConfig);
                boardConfig.pinConfig.ForEach((PinConfig pinConfig) =>
                {
                    if(!pinConfig.disabled && !String.IsNullOrEmpty(pinConfig.page_key))
                    {
                        JoyPageKeys.Add(pinConfig.page_key);
                        if (pinConfig.page_key.Length < 10)
                        {
                            HookKeyboard(pinConfig.page_key);
                        }
                    }
                });
            }
        }

        void SetGameControlListener()
        {
            var joysticks = DirectInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices);
            foreach (var joyInstance in joysticks)
            {
                try
                {
                    var joystick = new Joystick(DirectInput, joyInstance.InstanceGuid);
                    // Set the joystick to use buffered data acquisition, so that button events are stored in a buffer and can be retrieved later
                    joystick.Properties.BufferSize = 128;
                    // Acquire the joystick to gain exclusive control over it:
                    joystick.Acquire();
                    Joysticks.Add(joystick);
                } catch(Exception ex)
                {
                    ConsoleLog.Error("Joystick acquire error: " + ex.Message);
                }
            }
            JoyTimer.Elapsed += (sender, args) =>
            {
                Joysticks.ForEach(joystick =>
                {
                    try
                    {
                        joystick.Poll();
                        JoystickUpdate[] datas = joystick.GetBufferedData();
                        foreach (JoystickUpdate state in datas)
                        {
                            if (state.Value <= 0)
                            {
                                continue;
                            }
                            string joyId = joystick.Information.InstanceGuid.ToString() + ":" + state.Offset.ToString();
                            // Update textbox to idetntify joy id for GMT configuration (page_key)
                            if (textBoxJoystickListener.InvokeRequired)
                            {
                                Action safeWrite = delegate
                                {
                                    textBoxJoystickListener.Text = joyId;
                                };
                                Invoke(safeWrite);
                            }
                            else
                            {
                                textBoxJoystickListener.Text = joyId;
                            }
                            if (JoyPageKeys.Contains(joyId))
                            {
                                JoyListener_ButtonDown(joyId);
                            }
                        }
                    } catch (Exception ex)
                    {
                        ConsoleLog.Error("Joystick poll error: " + ex.Message);
                    }
                });
                
            };
            JoyTimer.AutoReset = true;
            JoyTimer.Enabled = true;
        }

        void HookKeyboard(string key)
        {
            try
            {
                // TODO: use SharpDX
                // Hack pour key listener: si le jeu démarre l'event est apparement perdu, d'où le timer
                GlobalKeyboardHook gkh = new GlobalKeyboardHook();
                Keys k = (Keys)Enum.Parse(typeof(Keys), key.ToUpper());
                gkh.HookedKeys.Add(k);
                System.Timers.Timer t = new System.Timers.Timer(5000);
                t.Elapsed += (sender, args) =>
                {
                    gkh.KeyDown -= KListener_KeyDown;
                    gkh.KeyDown += KListener_KeyDown;
                };
                t.AutoReset = true;
                t.Enabled = true;
                timerKeyListeners.Add(t);
            }
            catch(Exception ex)
            {
                ConsoleLog.Error("Key listener error for " + key + ": " + ex.Message);
            }
        }

        void KListener_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                GMTConfig.NextPage(e.KeyCode.ToString());
                ConsoleLog.Debug("Key event [Next page]: " + e.KeyCode.ToString());
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("Key page error: " + ex.Message);
            }
        }

        void JoyListener_ButtonDown(string buttonId)
        {
            try
            {
                GMTConfig.NextPage(buttonId);
                ConsoleLog.Debug("Joystick event [Next page]: " + buttonId);
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("Joystick page error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBoxJoystickListener.Text);
        }
    }
}
