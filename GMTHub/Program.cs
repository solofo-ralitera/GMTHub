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
        static TelemetryData Start(IGameProvider game)
        {
            try
            {
                return game.Loop();
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("ERROR: " + ex.Message);
                return new TelemetryData
                {
                    notfilled = true
                };
            }
        }

        static void Main(string[] args)
        {
            TelemetryData data;
            ICom com = new PortCom();
            IGameProvider game = new ScSSProvider();
            if (!game.Init())
            {
                ConsoleLog.Error("Game init error");
                return;
            }

            Console.WriteLine("GMTHGub started");
            if(!com.Scan())
            {
                ConsoleLog.Error("No arduino device found");
                Thread.Sleep(5000);
                Main(args);
                return;
            }

            while (true)
            {
                data = Start(game);
                if (data.notfilled)
                {
                    Thread.Sleep(3000);
                    continue;
                }

                // TODO: send ack/ready message from mc chip
                // Task.Delay(100);
                Thread.Sleep(100);
            }
        }
    }
}
