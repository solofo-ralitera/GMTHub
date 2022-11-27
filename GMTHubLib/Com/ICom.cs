using GMTHubLib.GameProvider;
using GMTHubLib.Models;
using GMTHubLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHubLib.Com
{
    public interface ICom
    {
        List<PortContainer> GetPorts();
        bool Scan();

        void SetConfig(GMTConfig config);

        void SetBlinker(Blinker blinker);
        void SendData(PortContainer portContainer, BoardConfig boardConfig);

        Task ProcessAllPorts(IGameProvider game);
    }
}
