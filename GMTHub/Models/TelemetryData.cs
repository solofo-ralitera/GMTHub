using GMTHub.Com;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://etcars.readthedocs.io/en/master/thedata.html

namespace GMTHub.Models
{
    public class TelemetryData
    {
        public bool notfilled = false;
        public ushort rpm { get; internal set; }
        public ushort rpm_max { get; internal set; }
        public float speed_ms { get; internal set; }
        public float speed_kph { get; internal set; }
        public float speed_Mph { get; internal set; }

        public sbyte gear { get; internal set; }

        public bool electric_on { get; internal set; }
        public bool engine_on { get; internal set; }
        public bool cruiseControl_on { get; internal set; }
        public float cruiseControl_value { get; internal set; }
        public bool differentialLock { get; internal set; }

        public bool parkingBrake { get; internal set; }
        public bool motorBrake { get; internal set; }
        public bool wipers { get; internal set; }
        public bool blinkerLeft { get; internal set; }
        public bool blinkerRight { get; internal set; }
        public bool hazardLight { get; internal set; }

        /// <summary>
        /// Intensity of the dashboard backlight as factor. <0,1>
        /// </summary>
        public float backLight { get; internal set; }
        public bool parkingLight { get; internal set; }
        public bool beamLowLight { get; internal set; }
        public bool beamHighLight { get; internal set; }
        public bool beaconLight { get; internal set; }
        public bool brakeLight { get; internal set; }
        public bool reverseLight { get; internal set; }

        /// <summary>
        /// 0: Off, 1: Dimmed, 2: Full
        /// </summary>
        public uint auxFront { get; internal set; }

        /// <summary>
        /// 0: Off, 1: Dimmed, 2: Full
        /// </summary>
        public uint auxRoof { get; internal set; }

        public float odometer { get; internal set; }
        public bool warning { get; internal set; }

        /// <summary>
        /// Amount of fuel currently in the tank
        /// </summary>
        public float fuel { get; internal set; }
        /// <summary>
        ///  Average consumption of the fuel in litres/100km
        /// </summary>
        public float fuel_averageConsumption { get; internal set; }
        public float fuel_capacity { get; internal set; }

        /// <summary>
        /// Estimated range of the current truck with current amount of fuel in km.
        /// </summary>
        public float fuel_range { get; internal set; }

        public ushort fuel_pct { get; internal set; }
        public bool fuel_warning { get; internal set; }

        /// <summary>
        /// Pressure of the oil in psi(pounds per square inch).
        /// </summary>
        public float oilPressure { get; internal set; }
        public bool oilPressure_warning { get; internal set; }
        /// <summary>
        /// The loose approximation of the temperature of the oil in degrees celsius.
        /// </summary>
        public float oilTemperature { get; internal set; }

        public float waterTemperature { get; internal set; }
        public bool waterTemperature_warning { get; internal set; }

        /// <summary>
        /// Amount of adBlue in litres. NOTE: This value will always be 0 in American Truck Simulator(ats).
        /// </summary>
        public float adblue { get; internal set; }
        public float adblue_capacity { get; internal set; }
        public ushort adblue_pct { get; internal set; }
        public bool adblue_warning { get; internal set; }

        /// <summary>
        ///  Pressure in the brake air tank in psi(pounds per square inch).
        /// </summary>
        public float airPressure { get; internal set; }
        public bool airPressure_warning { get; internal set; }
         
        public float batteryVoltage { get; internal set; }
        public bool batteryVoltage_warning { get; internal set; }

        public float speedLimit_ms { get; internal set; }
        public float speedLimit_kph { get; internal set; }
        public float speedLimit_Mph { get; internal set; }
        public bool speedLimit_warning { get; internal set; }

        /// <summary>
        /// Mass of the cargo in kilograms
        /// </summary>
        public float cargoMass { get; internal set; }
        public float cargoMass_ton { get; internal set; }

        /// <summary>
        /// Navigation distance remaining until the next waypoint is hit OR distance remaining until the company of the delivery is reached. 
        /// Represented in meters. 
        /// NOTE: When electronics are disabled or when route advisor is completely disabled, this value will ALWAYS be 0.
        /// </summary>
        public float distance { get; internal set; }


        public Blinker blinker;
        public string ProcessOutput(BoardConfig boardConfig)
        {
            string cmd = "";
            boardConfig.pinConfig.ForEach(pinConfig =>
            {
                if (pinConfig.disabled) return;
                if (String.IsNullOrEmpty(pinConfig.output_type))
                {
                    ConsoleLog.Error($"Process error on pin {pinConfig.pin}: output_type is not defined");
                    return;
                }
                if (String.IsNullOrEmpty(pinConfig.data_binding))
                {
                    ConsoleLog.Error($"Process error on pin {pinConfig.pin}: data_binding is not defined");
                    return;
                }
                cmd += ProcessPin(pinConfig);
            });
            return cmd;
        }

        public string ProcessPin(PinConfig pinConfig) {
            try
            {
                switch (pinConfig.output_type)
                {
                    case "servo":
                        return ProcessServo(pinConfig);
                    case "digital":
                        return ProcessDigital(pinConfig);
                    case "analog":
                        return ProcessAnalog(pinConfig);
                    case "max7seg":
                        return ProcessMax7seg(pinConfig);
                    case "analogdisc":
                        return ProcessAnalogDisc(pinConfig);
                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                return "";
            }
        }

        public bool BlinkIf(PinConfig pinConfig)
        {
            if (String.IsNullOrEmpty(pinConfig.blink_if))
            {
                return false;
            }
            // TODO mettre dans une fonction
            // Evaluate range
            if(pinConfig.blink_if.IndexOf('|') > 0)
            {
                string[] range = pinConfig.blink_if.Split('|');
                float fValue = StringUtils.ParseFloat((this.GetType().GetProperty(range[0]).GetValue(this, null)).ToString());
                float r1 = StringUtils.ParseFloat((range[1]).ToString());
                float r2 = StringUtils.ParseFloat((range[2]).ToString());
                return fValue >= r1 && fValue <= r2; // TODO evaluate range
            } else
            {
                var value = this.GetType().GetProperty(pinConfig.blink_if).GetValue(this, null);
                return (bool)value;
            }
        }

        // Blink for 
        public int Blink(PinConfig pinConfig, int i)
        {
            if (pinConfig.blink == 0) return i;
            if (!BlinkIf(pinConfig)) return i;
            if (!blinker.GetStatus(pinConfig.blink)) return 0;
            return i;
        }

        public string Blink(PinConfig pinConfig, string val)
        {
            if (pinConfig.blink == 0) return val;
            if (!BlinkIf(pinConfig)) return val;
            if (!blinker.GetStatus(pinConfig.blink)) return "";
            return val;
        }

        // [output code: s (char(1))][pin number int(2)][pin value int(3)]
        // s[0-9]{2}[0-9]{3}
        public string ProcessServo(PinConfig pinConfig)
        {
            float value = StringUtils.ParseFloat(this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null).ToString());

            // Physical limite of the servo
            ushort servoMaxRange = pinConfig.servo_relative_max > 0 ? 
                Math.Min(pinConfig.servo_max_range, pinConfig.servo_relative_max)
                : pinConfig.servo_max_range;

            value = Math.Max(value, pinConfig.data_offset);

            // Math.Min: pour ne pas dépasser le range max du servo
            ushort angle = (ushort) Math.Min(
                pinConfig.servo_relative_min + Math.Round(value / pinConfig.servo_step_value), 
                servoMaxRange
            );
            if(pinConfig.servo_reverse_rotation)
            {
                angle = (ushort)(servoMaxRange - angle);
            }
            // servoMaxRange - angle: pour avoir un sens gauche vers droite
            return $"s{pinConfig.pin.ToString("00")}{(angle).ToString("000")}";
        }

        public string ProcessDigital(PinConfig pinConfig)
        {
            var value = this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null);
            if(value is bool)
            {
                bool bValue = (bool)value;
                return $"d{pinConfig.pin.ToString("00")}{Blink(pinConfig, (bValue ? 1 : 0))}";
            }
            float fValue = StringUtils.ParseFloat(value.ToString().ToString());

            float maxValue = pinConfig.data_max_value > 0 ? pinConfig.data_max_value : fValue;
            if (fValue >= pinConfig.data_min_value && fValue <= pinConfig.data_max_value)
            {
                return $"d{pinConfig.pin.ToString("00")}{Blink(pinConfig, 1)}";
            }
            return $"d{pinConfig.pin.ToString("00")}0";
        }

        public string ProcessAnalog(PinConfig pinConfig)
        {
            int value = (int)Math.Max(
                Convert.ToInt32(this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null)),
                pinConfig.data_offset
            );
            float dataMaxValue = pinConfig.data_max_value == 0 ? 100f : pinConfig.data_max_value;
            // Si la donnée est < min: ignore
            if (value < pinConfig.data_min_value)
            {
                value = 0;
            }
            if (value > dataMaxValue)
            {
                dataMaxValue = value;
            }
            int analogValue = Math.Min(
                Math.Max(
                    (int) RangeUtils.Remap(value, pinConfig.data_min_value, dataMaxValue, 0, 255),
                    0
                ), 
                255
            );
            return $"a{pinConfig.pin.ToString("00")}{Blink(pinConfig, analogValue).ToString("000")}";
        }

        /**
         * maxType 1 : 7seg
         * maxType 2 : Matrix
         * */
        public string ProcessMax7seg(PinConfig pinConfig)
        {
            string value = this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null).ToString();
            ushort digitLength = pinConfig.digit_length;
            // Pour le 7 seg: si value contient un decimal point: rajoute 1 à digit_length car le DP ny prend pas la place de un digit sur le 7seg
            if (pinConfig.max_type == 0 && (value.IndexOf('.') > 0 || value.IndexOf(',') > 0))
            {
                digitLength++;
            }
            if(value.Length > digitLength)
            {
                value = value.Substring(0, digitLength);
            }
            if (pinConfig.reverse_digit)
            {
                value = StringUtils.Reverse(value);
            }
            // Matrix display
            if(pinConfig.max_type == 1)
            {
                // transform la valeur en clé si necessaire
                int iValue;
                int.TryParse(value, out iValue);
                if(!String.IsNullOrEmpty(pinConfig.matrix_value_correspondance))
                {
                    // Recherche si correspondance clé valeur configuré
                    string[] aCorres = pinConfig.matrix_value_correspondance.Split(',');
                    foreach (string corres in aCorres)
                    {
                        string[] ac = corres.Split(':');
                        if(value == ac[0])
                        {
                            int.TryParse(ac[1], out iValue);
                            break;
                        }
                    }
                }
                iValue = Math.Min(iValue, 29); // 29 correspond au caractères vide du Matrix 
                value = iValue.ToString();
            }
            return $"m{pinConfig.pin.ToString("00")}{pinConfig.din_pin.ToString("00")}{pinConfig.cs_pin.ToString("00")}{pinConfig.clk_pin.ToString("00")}{pinConfig.display_offset.ToString("00")}{pinConfig.max_type}{digitLength.ToString("00")}{Blink(pinConfig, value).PadLeft(digitLength, ' ')}";
        }
        
        public string ProcessAnalogDisc(PinConfig pinConfig)
        {
            if (String.IsNullOrEmpty(pinConfig.discrete_value))
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: discrete_value is not defined");
                return "";
            }

            string[] properties = pinConfig.data_binding.Split('|');
            string[] discreteValues = pinConfig.discrete_value.Split('|');
            if(properties.Length != discreteValues.Length)
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: data_binding and discrete_value must have the same number of item");
                return "";
            }
            for (int i = 0; i < properties.Length; i++)
            {
                if(Convert.ToBoolean(this.GetType().GetProperty(properties[i]).GetValue(this, null)))
                {
                    return $"a{pinConfig.pin.ToString("00")}{Blink(pinConfig, discreteValues[i]).PadLeft(3, '0')}";
                }
            }
            return $"a{pinConfig.pin.ToString("00")}000";
        }
    }
}
