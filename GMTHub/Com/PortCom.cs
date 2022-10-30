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
        public SerialPort port;
    }

    public class PortCom: ICom
    {
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
                Thread.Sleep(500); // Wait for arduino respond
                string response = ReadMessage(sp);
                if(response.StartsWith("ack_gmtscan_"))
                {
                    string boardType = response.Replace("ack_gmtscan_", "").Trim().ToUpper();
                    ConsoleLog.Success("---- " + port + " " + boardType + " connected");
                    Ports.Add(new PortContainer
                    {
                        type = boardType,
                        port = sp,
                    });
                } else
                {
                    sp.Close();
                    sp.Dispose();
                }
            }
            return Ports.Count != 0;
        }

        protected string ReadMessage(SerialPort sp)
        {
            try
            {
                return sp.ReadLine().Trim();
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("SerialPort Error - Read message: " + ex.Message);
                return "";
            }
        }

        public void SendMessage(SerialPort sp, string message)
        {
            try
            {
                if (!sp.IsOpen) sp.Open();
                sp.Write(message + "#");
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("SerialPort Error - Send message: " + ex.Message);
            }
        }
    }
}
