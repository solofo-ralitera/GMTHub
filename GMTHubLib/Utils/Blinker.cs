using GMTHubLib.Com;
using GMTHubLib.GameProvider;
using GMTHubLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GMTHubLib.Utils
{
    public class SharedBoardData
    {
        public bool consumed = false;
        public string data = "";
    }
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

        public Dictionary<byte, SharedBoardData> BoardData = new Dictionary<byte, SharedBoardData>();

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
            try
            {
                BlinkTimer_1.Stop();
                BlinkTimer_1.Dispose();
            }
            catch (Exception) { };

            try
            {
                BlinkTimer_2.Stop();
                BlinkTimer_2.Dispose();
            }
            catch (Exception) { };

            try
            {
                BlinkTimer_3.Stop();
                BlinkTimer_3.Dispose();
            }
            catch (Exception) { };

            try
            {
                DataTask.Dispose();
            }
            catch (Exception) { };
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

        // Contient le timestamp en millis de la dernière date de data envoyé
        public static Dictionary<byte, Stopwatch> PinCache = new Dictionary<byte, Stopwatch>();
        public static bool IsCached(PinConfig pinConfig)
        {
            if (pinConfig.cache == 0) return false;
            if (!PinCache.ContainsKey(pinConfig.pin))
            {
                PinCache[pinConfig.pin] = System.Diagnostics.Stopwatch.StartNew();
            }
            if (pinConfig.cache < PinCache[pinConfig.pin].ElapsedMilliseconds)
            {
                PinCache[pinConfig.pin].Restart();
                return false;
            }
            return true;
        }

        public void PullData()
        {
            while(true)
            {
                try
                {
                    GameData = Game.GetData();
                    if (GameData.notfilled)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                    // Process data for all boards
                    Com.GetPorts().ForEach((PortContainer portContainer) =>
                    {
                        BoardConfig boardConfig = Config.GetBoardConfig(portContainer.number);
                        if(BoardData.ContainsKey(portContainer.number) && BoardData[portContainer.number].consumed == false)
                        {
                            // Raf tant que le dernier message n'a pas été consomé
                            return;
                        }
                        BoardData[portContainer.number] = new SharedBoardData()
                        {
                            consumed = false,
                            data = GameData.ProcessOutput(boardConfig)
                        };
                    });
                    Thread.Sleep(16); // Refrech ~ 60fps
                    // Task.Delay(16);
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
