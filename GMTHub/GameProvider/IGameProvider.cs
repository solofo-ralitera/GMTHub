using GMTHub.Models;
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

        TelemetryData Loop();
    }
}
