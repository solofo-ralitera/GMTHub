using GMTHub.Com;
using GMTHub.GameProvider;
using GMTHub.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GMTHub.Utils
{
    /// <summary>
    /// Mangage all shared timer, including refreshing game data
    /// </summary>
    public class Blinker
    {
        public System.Timers.Timer BlinkTimer_1;
        public bool TimerStatus_1 = true;

        public System.Timers.Timer BlinkTimer_2;
        public bool TimerStatus_2 = true;

        public System.Timers.Timer BlinkTimer_3;
        public bool TimerStatus_3 = true;

        public Dictionary<byte, string> BoardData = new Dictionary<byte, string>();

        protected GMTConfig Config;
        protected ICom Com;
        protected IGameProvider Game;
        public TelemetryData GameData;
        protected Task DataTask;
        public Blinker(IGameProvider game, ICom com, GMTConfig config)
        {
            BlinkTimer_1 = new System.Timers.Timer(1000);
            BlinkTimer_1.Elapsed += (sender, args) => {
                TimerStatus_1 = !TimerStatus_1;
            };
            BlinkTimer_1.AutoReset = true;
            BlinkTimer_1.Enabled = true;


            BlinkTimer_2 = new System.Timers.Timer(500);
            BlinkTimer_2.Elapsed += (sender, args) => {
                TimerStatus_2 = !TimerStatus_2;
            };
            BlinkTimer_2.AutoReset = true;
            BlinkTimer_2.Enabled = true;


            BlinkTimer_3 = new System.Timers.Timer(150);
            BlinkTimer_3.Elapsed += (sender, args) => {
                TimerStatus_3 = !TimerStatus_3;
            };
            BlinkTimer_3.AutoReset = true;
            BlinkTimer_3.Enabled = true;

            Com = com;
            Config = config;

            // Refresh game data environ ~ 60fps
            Game = game;
            DataTask = Task.Factory.StartNew(() => PullData());
        }

        ~Blinker()
        {
            BlinkTimer_1.Stop();
            BlinkTimer_1.Dispose();

            BlinkTimer_2.Stop();
            BlinkTimer_2.Dispose();

            BlinkTimer_3.Stop();
            BlinkTimer_3.Dispose();

            DataTask.Dispose();
        }

        public bool GetStatus(ushort i)
        {
            switch(i)
            {
                case 1:
                    return TimerStatus_1;
                case 2:
                    return TimerStatus_2;
                case 3:
                    return TimerStatus_3;
                default:
                    return false;
            }
        }

        public void PullData()
        {
            while(true)
            {
                try
                {
                    // Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                    GameData = Game.GetData();
                    if (GameData.notfilled)
                    {
                        // watch.Stop();
                        Thread.Sleep(1000);
                        continue;
                    }
                    // Process data for all boards
                    Com.GetPorts().ForEach((PortContainer portContainer) =>
                    {
                        BoardConfig boardConfig = Config.GetBoardConfig(portContainer.number);
                        BoardData[portContainer.number] = GameData.ProcessOutput(boardConfig);
                    });
                    // watch.Stop();
                    Thread.Sleep(16); // Refrech ~ 60fps
                }
                catch (Exception ex)
                {
                    ConsoleLog.Error("Blinker Error: " + ex.Message);
                    Thread.Sleep(16); // Refrech ~ 60fps
                }
            }
        }
    }
}
