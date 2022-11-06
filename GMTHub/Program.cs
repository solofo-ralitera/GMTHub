using GMTHub.Com;
using GMTHub.GameProvider;
using GMTHub.Models;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMTHub
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GMTConfig config = new GMTConfig();
            IGameProvider game = new ScSSProvider();
            if (!game.Init())
            {
                ConsoleLog.Error("Game init error");
                return;
            }

            ICom com = new PortCom();
            com.SetConfig(config);

            Blinker blinker = new Blinker(game, com, config);


            Console.WriteLine("GMTHub started");
            if(!com.Scan())
            {
                ConsoleLog.Error("No arduino device found");
                Thread.Sleep(2000);
                Main(args);
                return;
            }
            
            com.SetBlinker(blinker);
            game.SetBlinker(blinker);
            Task task = com.ProcessAllPorts(game);
            task.Wait();
            Thread.Sleep(1000);
            Main(args); // Reboucle si arrêt des tasks
        }
    }
}
