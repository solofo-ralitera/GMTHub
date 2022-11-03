﻿using GMTHub.GameProvider;
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
        bool Scan();

        void SetConfig(GMTConfig config);

        void SendData(TelemetryData data);

        Task ProcessAllPorts(IGameProvider game);
    }
}