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
            GMTConfig config = new GMTConfig();
            
            ICom com = new PortCom();
            com.SetConfig(config);

            IGameProvider game = new ScSSProvider(); // TODO: ici switch game
            if (!game.Init())
            {
                ConsoleLog.Error("Game init error");
                return;
            }

            Console.WriteLine("GMTHub started");
            if(!com.Scan())
            {
                ConsoleLog.Error("No arduino device found");
                Thread.Sleep(5000);
                Main(args);
                return;
            }

            TelemetryData data;
            while (true)
            {
                data = Start(game);
                if (data.notfilled)
                {
                    Thread.Sleep(3000);
                    continue;
                }

                // Wait for Com
                // TODO move le wait juste avant l'envoi au Serial
                // while(!com.WaitReady()) {}
                com.SendData(data);
            }
        }
    }
}
