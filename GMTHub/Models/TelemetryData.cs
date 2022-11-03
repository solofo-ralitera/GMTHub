﻿using GMTHub.Com;
using GMTHub.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        public bool warning { get; internal set; }

        public float fuel { get; internal set; }
        public float fuel_averageConsumption { get; internal set; }
        public float fuel_capacity { get; internal set; } // Type à confirmer
        public float fuel_range { get; internal set; } // Type à confirmer
        public byte fuel_pct { get; internal set; } // Type à confirmer
        public bool fuel_warning { get; internal set; }

        public float oilPressure { get; internal set; }
        public bool oilPressure_warning { get; internal set; }

        public float oilTemperature { get; internal set; }

        public float waterTemperature { get; internal set; }
        public bool waterTemperature_warning { get; internal set; }

        public float adblue { get; internal set; }
        public float adblue_capacity { get; internal set; }
        public byte adblue_pct { get; internal set; }
        public bool adblue_warning { get; internal set; }
        
        public float airPressure { get; internal set; }
        public bool airPressure_warning { get; internal set; }
         
        public float batteryVoltage { get; internal set; }
        public bool batteryVoltage_warning { get; internal set; }

        public float speed_limit { get; internal set; }

        public string ProcessOutput(BoardConfig boardConfig)
        {
            string cmd = "";
            boardConfig.pinConfig.ForEach(pinConfig =>
            {
                if (CheckDisabled(pinConfig)) return;
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

        protected bool CheckDisabled(PinConfig pinConfig)
        {
            return pinConfig.disabled;
        }

        // [output code: s (char(1))][pin number int(2)][pin value int(3)]
        // s[0-9]{2}[0-9]{3}
        public string ProcessServo(PinConfig pinConfig)
        {
            int value = Convert.ToInt32(this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null));

            // Physical limite of the servo
            ushort servoMaxRange = pinConfig.servo_relative_max > 0 ? Math.Min(pinConfig.servo_max_range, pinConfig.servo_relative_max) : pinConfig.servo_max_range;

            value = (int) Math.Max(value, pinConfig.data_offset);

            // Math.Min: pour ne pas dépasser le range max du servo
            ushort angle = (ushort) Math.Min(
                pinConfig.servo_relative_min + Math.Round((float)value / pinConfig.servo_step_value), 
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
                return $"d{pinConfig.pin.ToString("00")}{((bool)value ? "1" : "0")}";
            }
            float fValue;
            Single.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out fValue);

            float maxValue = pinConfig.data_max_value > 0 ? pinConfig.data_max_value : fValue;
            if (fValue >= pinConfig.data_min_value && fValue <= pinConfig.data_max_value)
            {
                return $"d{pinConfig.pin.ToString("00")}1";
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
            return $"a{pinConfig.pin.ToString("00")}{analogValue.ToString("000")}";
        }

        public string ProcessMax7seg(PinConfig pinConfig)
        {
            string value = this.GetType().GetProperty(pinConfig.data_binding).GetValue(this, null).ToString();
            if(value.Length > pinConfig.digit_length)
            {
                value = value.Substring(0, pinConfig.digit_length);
            }
            if (pinConfig.reverse_digit)
            {
                value = StringUtils.Reverse(value);
            }
            return $"m{pinConfig.pin.ToString("00")}{pinConfig.din_pin.ToString("00")}{pinConfig.cs_pin.ToString("00")}{pinConfig.clk_pin.ToString("00")}{pinConfig.display_offset.ToString("00")}{pinConfig.digit_length.ToString("00")}{value.PadLeft(pinConfig.digit_length, ' ')}";
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
                    return $"a{pinConfig.pin.ToString("00")}{discreteValues[i].PadLeft(3, '0')}";
                }
            }
            return $"a{pinConfig.pin.ToString("00")}000";
        }
    }
}
