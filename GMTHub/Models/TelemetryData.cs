using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMTHub.Models
{
    public class TelemetryData
    {
        public bool notfilled = false;
        public ushort rpm { get; internal set; }
        public ushort rpm_max { get; internal set; }
        public short speed { get; internal set; }
        public sbyte gear { get; internal set; }

        public bool electric_on { get; internal set; }
        public bool engine_on { get; internal set; }

        public bool parkingBrake { get; internal set; }

        public bool blinkerLeft { get; internal set; }
        public bool blinkerRight { get; internal set; }
        public bool hazardLight { get; internal set; }

        public float odometer { get; internal set; }

        public ushort fuel { get; internal set; }
        public float fuel_averageConsumption { get; internal set; }
        public float fuel_range { get; internal set; } // Type à confirmer
        public bool fuel_warning { get; internal set; }

        public byte oil_temperature { get; internal set; }
        public bool oil_warning { get; internal set; }

        public byte water_temperature { get; internal set; }
        public bool water_warning { get; internal set; }

        public bool adblue { get; internal set; }
        public bool adblue_warning { get; internal set; }
        
        public float airPressure { get; internal set; }
        public bool airPressure_warning { get; internal set; }

        public bool battery_warning { get; internal set; }

        public string ProcessOutput(List<PinConfig> pinConfig)
        {
            pinConfig.ForEach(config =>
            {
                // config.pin
            });
            return "";
        }
    }
}
