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
        public ushort speed_ms { get; internal set; }
        public ushort speed_kph { get; internal set; }
        public ushort speed_Mph { get; internal set; } // Miles per hour

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

        public string ProcessOutput(BoardConfig boardConfig)
        {
            string cmd = "";
            boardConfig.pinConfig.ForEach(pinConfig =>
            {
                cmd += ProcessPin(pinConfig);
            });
            return cmd;
        }

        public string ProcessPin(PinConfig pinConfig) {
            string outputType = DictUtils.GetString(pinConfig.config, "output_type", "");
            if (String.IsNullOrEmpty(outputType))
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: output_type is not defined");
                return "";
            }
            string propertyKey = DictUtils.GetString(pinConfig.config, "data_binding", "");
            if(String.IsNullOrEmpty(propertyKey))
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: data_binding is not defined");
                return "";
            }
            switch(outputType)
            {
                case "servo":
                    try
                    {
                        var value = this.GetType().GetProperty(propertyKey).GetValue(this, null);
                        return ProcessServo(Convert.ToInt32(value), pinConfig);
                    } catch (Exception ex)
                    {
                        ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                        return "";
                    }
                case "digital":
                    try
                    {
                        var value = this.GetType().GetProperty(propertyKey).GetValue(this, null);
                        return ProcessDigital(Convert.ToBoolean(value), pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                        return "";
                    }
                case "analog":
                    try
                    {
                        var value = this.GetType().GetProperty(propertyKey).GetValue(this, null);
                        return ProcessAnalog(Convert.ToInt32(value), pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                        return "";
                    }
                case "max72xx":
                    try
                    {
                        var value = this.GetType().GetProperty(propertyKey).GetValue(this, null);
                        return ProcessMax72xx(value.ToString(), pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                        return "";
                    }
                case "analogdisc":
                    try
                    {
                        return ProcessAnalogDisc(pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Process error on Pin " + pinConfig.pin + ": " + ex.Message);
                        return "";
                    }
                default:
                    return "";
            }
            return "";
        }

        protected bool CheckDisabled(PinConfig pinConfig)
        {
            bool disabled = false;
            bool.TryParse(DictUtils.GetString(pinConfig.config, "disabled"), out disabled);
            return disabled;
        }

        // [output code: s (char(1))][pin number int(2)][pin value int(3)]
        // s[0-9]{2}[0-9]{3}
        public string ProcessServo(int value, PinConfig pinConfig)
        {
            if (CheckDisabled(pinConfig)) return "";

            // Physical limite of the servo
            ushort servoMaxRange = 90;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "servo_max_range"), out servoMaxRange);

            float servoStepValue = 1f;
            Single.TryParse(DictUtils.GetString(pinConfig.config, "servo_step_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out servoStepValue);

            // Relative limit of the servo
            ushort servoRelativeAngleMin = 0;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "servo_relative_min"), out servoRelativeAngleMin);

            // Relative limit of the servo
            ushort servoRelativeAngleMax = servoMaxRange;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "servo_relative_max"), out servoRelativeAngleMax);

            // Inverse le sens de rotation
            bool servoReverseRotation = false;
            bool.TryParse(DictUtils.GetString(pinConfig.config, "servo_reverse_rotation"), out servoReverseRotation);

            int dataOffset = 0;
            int.TryParse(DictUtils.GetString(pinConfig.config, "data_offset"), out dataOffset);
            value = Math.Max(value, dataOffset);

            if(servoRelativeAngleMax > 0)
            {
                servoMaxRange = Math.Min(servoMaxRange, servoRelativeAngleMax);
            }

            // Math.Min: pour ne pas dépasser le range max du servo
            ushort angle = (ushort) Math.Min(
                servoRelativeAngleMin + Math.Round((float)value / servoStepValue), 
                servoMaxRange
            );
            if(servoReverseRotation)
            {
                angle = (ushort)(servoMaxRange - angle);
            }
            // servoMaxRange - angle: pour avoir un sens gauche vers droite
            string result = $"s{pinConfig.pin.ToString("00")}{(angle).ToString("000")}";
            return result;
        }

        public string ProcessDigital(bool value, PinConfig pinConfig)
        {
            if (CheckDisabled(pinConfig)) return "";

            return $"d{pinConfig.pin.ToString("00")}{(value ? 1 : 0).ToString("0")}";
        }

        public string ProcessAnalog(float value, PinConfig pinConfig)
        {
            if (CheckDisabled(pinConfig)) return "";

            float dataMinValue = 0;
            Single.TryParse(DictUtils.GetString(pinConfig.config, "data_min_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dataMinValue);

            float dataMaxValue = 100;
            Single.TryParse(DictUtils.GetString(pinConfig.config, "data_max_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dataMaxValue);

            float dataOffset = 0f;
            Single.TryParse(DictUtils.GetString(pinConfig.config, "data_offset"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dataOffset);
            value = Math.Max(value, dataOffset);

            // Si la donnée est < min: ignore
            if (value < dataMinValue)
            {
                value = 0;
            }
            if (value > dataMaxValue)
            {
                dataMaxValue = value;
            }
            int analogValue = (int)RangeUtils.Remap(value, dataMinValue, dataMaxValue, 0, 255);
            analogValue = Math.Max(analogValue, 0);
            analogValue = Math.Min(analogValue, 255);
            return $"a{pinConfig.pin.ToString("00")}{analogValue.ToString("000")}";
        }

        public string ProcessMax72xx(string value, PinConfig pinConfig)
        {
            if (CheckDisabled(pinConfig)) return "";

            ushort csPin = 9;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "cs_pin"), out csPin);

            ushort clkPin = 5;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "clk_pin"), out clkPin);

            ushort digitLength = 4;
            ushort.TryParse(DictUtils.GetString(pinConfig.config, "digit_length"), out digitLength);

            bool reverseDigit = false;
            bool.TryParse(DictUtils.GetString(pinConfig.config, "reverse_digit"), out reverseDigit);
            if(value.Length > digitLength)
            {
                value = value.Substring(0, digitLength);
            }
            if (reverseDigit)
            {
                value = StringUtils.Reverse(value);
            }
            return $"m{pinConfig.pin.ToString("00")}{csPin.ToString("00")}{clkPin.ToString("00")}{digitLength.ToString("00")}{value.PadLeft(digitLength, ' ')}";
        }

        public string ProcessAnalogDisc(PinConfig pinConfig)
        {
            if (CheckDisabled(pinConfig)) return "";

            string propertyKey = DictUtils.GetString(pinConfig.config, "data_binding", "");
            if (String.IsNullOrEmpty(propertyKey))
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: data_binding is not defined");
                return "";
            }

            string discreteValue = DictUtils.GetString(pinConfig.config, "discrete_value", "");
            if (String.IsNullOrEmpty(propertyKey))
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: discrete_value is not defined");
                return "";
            }

            string[] properties = propertyKey.Split('|');
            string[] discreteValues = discreteValue.Split('|');
            if(properties.Length != discreteValues.Length)
            {
                ConsoleLog.Error($"Process error on pin {pinConfig.pin}: data_binding and discrete_value must have the same number of item");
                return "";
            }
            for (int i = 0; i < properties.Length; i++)
            {
                if(Convert.ToBoolean(this.GetType().GetProperty(properties[i]).GetValue(this, null)))
                {
                    return $"a{pinConfig.pin.ToString("00")}{discreteValues[i].PadLeft(3, '0')}";
                }
            }
            return $"a{pinConfig.pin.ToString("00")}000";
        }
    }
}
