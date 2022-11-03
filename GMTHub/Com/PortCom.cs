using GMTHub.GameProvider;
using GMTHub.Models;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GMTHub.Com
{
    public class PortContainer
    {
        public string type; // Type of arduino board (NANO, UNO)
        public byte number; // Number of the arduino board on this port
        public SerialPort port;
    }

    public class PortCom: ICom
    {
        protected GMTConfig Config;
        public List<PortContainer> Ports = new List<PortContainer>();

        ~PortCom()
        {
            Ports.ForEach((PortContainer port) =>
            {
                try
                {
                    port.port.Close();
                    port.port.Dispose();
                }
                catch (Exception ex)
                {
                    // Raf
                }
            });
        }

        public void SetConfig(GMTConfig config)
        {
            Config = config;
        }

        protected SerialPort InitSerialPort(string portName)
        {
            SerialPort sp = new SerialPort();
            sp.PortName = portName;
            sp.BaudRate = 9600; // Default on Arduino
            sp.ReadTimeout = 1000;
            sp.WriteTimeout = 1000;
            return sp;
        }

        public bool Scan()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                ConsoleLog.Info("Scanning " + port);
                SerialPort sp = InitSerialPort(port);
                SendMessage(sp, "gmtscan");
                Thread.Sleep(100); // Wait for arduino to respond
                string response = ReadMessage(sp);
                if(response.StartsWith("ack_gmtscan_"))
                {
                    string boardType = response.Replace("ack_gmtscan_", "").Trim().ToUpper();
                    string[] aBoard = boardType.Split('-');
                    try
                    {
                        byte boardNumber = Byte.TryParse(aBoard[1], out boardNumber) ? boardNumber : (byte)1;
                        if(!Config.boardHasConfig(boardNumber))
                        {
                            throw new Exception("Board " + boardType + " has no configuration in GMTHub.ini");
                        }
                        BoardConfig boardConfig = Config.GetBoardConfig(boardNumber);
                        Ports.Add(new PortContainer
                        {
                            type = aBoard[0],
                            number = boardNumber,
                            port = sp,
                        });
                        ConsoleLog.Success($"---- {port} BOARD {boardConfig.name} ({boardNumber}) connected");
                    } 
                    catch (Exception e)
                    {
                        ConsoleLog.Error("---- " + port + " " + boardType + " ERROR: "  + e.Message);
                        sp.Close();
                        sp.Dispose();
                    }
                }
                else
                {
                    sp.Close();
                    sp.Dispose();
                }
            }
            return Ports.Count != 0;
        }

        public async Task ProcessAllPorts(IGameProvider game)
        {
            var tasks = new List<Task>();
            Ports.ForEach((PortContainer portContainer) =>
            {
                BoardConfig boardConfig = Config.GetBoardConfig(portContainer.number);
                tasks.Add(Task.Factory.StartNew((Object obj) =>
                {
                    var data = (dynamic)obj;
                    ProcessPort(data.game, data.boardConfig);
                }, new { game = game, boardConfig = boardConfig }));

            });
            await Task.WhenAll(tasks);
        }

        public void ProcessPort(IGameProvider game, BoardConfig boardConfig)
        {
            TelemetryData data;
            while (true)
            {
                data = game.GetData();
                if (data.notfilled)
                {
                    Thread.Sleep(boardConfig.refreshDelay);
                    continue;
                }
                SendData(data);
                Thread.Sleep(boardConfig.refreshDelay);
            }
        }

        protected string ReadMessage(SerialPort sp)
        {
            try
            {
                return sp.ReadLine().Trim();
            }
            catch (Exception ex)
            {
                ConsoleLog.Error(sp.PortName + " Error - Read message: " + ex.Message);
                return "";
            }
        }

        public void SendMessage(SerialPort sp, string message)
        {
            try
            {
                if (!sp.IsOpen) sp.Open();
                sp.Write(message + "#");
                ConsoleLog.Debug("SerialPort  message: " + message);
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("SerialPort Error - Send message: " + ex.Message);
            }
        }

        public void SendData(TelemetryData data)
        {
            // TODO send async each board
            Ports.ForEach((PortContainer portContainer) =>
            {
                try
                {
                    BoardConfig boardConfig = Config.GetBoardConfig(portContainer.number);
                    string cmd = data.ProcessOutput(boardConfig);
                    SendMessage(portContainer.port, $":{cmd}:");
                }
                catch (Exception ex)
                {
                    ConsoleLog.Error("SerialPort "+ portContainer.port.PortName + " Error - Send Data : " + ex.Message);
                }
            });
        }
    }
}
