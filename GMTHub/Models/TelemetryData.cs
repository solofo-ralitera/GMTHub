using GMTHub.Com;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public float fuel { get; internal set; }
        public float fuel_averageConsumption { get; internal set; }
        public float fuel_capacity { get; internal set; } // Type à confirmer
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

        public string ProcessOutput(List<PinConfig> pinConfigs)
        {
            string cmd = "";
            pinConfigs.ForEach(pinConfig =>
            {
                cmd += ProcessPin(pinConfig);
            });
            return cmd;
        }

        public string ProcessPin(PinConfig pinConfig) {
            string outputType = pinConfig.config["output_type"];
            string protertyKey = pinConfig.config["data_binding"];
            var value = this.GetType().GetProperty(protertyKey).GetValue(this, null);
            switch(outputType)
            {
                case "servo":
                    try
                    {
                        return ProcessServo(Convert.ToInt32(value), pinConfig);
                    } catch (Exception ex)
                    {
                        ConsoleLog.Error("Process servo error on value " + value.ToString() + ": " + ex.Message);
                        return "";
                    }
                case "digital":
                    try
                    {
                        return ProcessDigital(Convert.ToBoolean(value), pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Process servo error on value " + value.ToString() + ": " + ex.Message);
                        return "";
                    }
                default:
                    return "";
            }
            return "";
        }

        // [output code: s (char(1))][pin number int(2)][pin value int(3)]
        // s[0-9]{2}[0-9]{3}
        public string ProcessServo(int value, PinConfig pinConfig)
        {
            ushort servoMaxRange;
            ushort.TryParse(pinConfig.config["servo_max_range"], out servoMaxRange);

            float servoStepValue = float.Parse(pinConfig.config["servo_step_value"], CultureInfo.InvariantCulture);
            ushort angle = Math.Min((ushort) Math.Round((float)value / servoStepValue), servoMaxRange);
            
            string result = $"s{pinConfig.pin.ToString("00")}{angle.ToString("000")}";
            return result;
        }

        public string ProcessDigital(bool value, PinConfig pinConfig)
        {
            string result = $"d{pinConfig.pin.ToString("00")}{(value ? 1 : 0).ToString("000")}";
            return result;
        }
    }
}
