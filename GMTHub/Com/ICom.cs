using GMTHub.GameProvider;
using GMTHub.Models;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.Com
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
