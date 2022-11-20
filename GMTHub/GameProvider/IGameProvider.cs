using GMTHub.Models;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.GameProvider
{
    public interface IGameProvider
    {
        bool Init();

        string GetGameName();

        TelemetryData GetData();

        void SetBlinker(Blinker blinker);
    }
}
