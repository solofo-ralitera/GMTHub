using GMTHubLib.Com;
using GMTHubLib.GameProvider;
using GMTHubLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMTHubLib
{
    public class GMTHubLib
    {
        public static void Start()
        {
            IGameProvider game = new ScSSProvider();
            if (!game.Init())
            {
                ConsoleLog.Error("Game init error");
                return;
            }
            GMTConfig config = new GMTConfig(game.GetGameName());

            ICom com = new PortCom();
            com.SetConfig(config);

            Blinker blinker = new Blinker(game, com, config);

            Console.WriteLine("GMTHub started");
            if (!com.Scan())
            {
                ConsoleLog.Error("No arduino device found");
                Thread.Sleep(2000);
                Start();
                return;
            }

            com.SetBlinker(blinker);
            game.SetBlinker(blinker);
            Task task = com.ProcessAllPorts(game);
            task.Wait();
            Thread.Sleep(1000);
            Start(); // Reboucle si arrêt des tasks
        }
    }
}
