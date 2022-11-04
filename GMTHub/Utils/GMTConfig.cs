using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace GMTHub.Utils
{
    public class BoardConfig
    {
        public string name;
        public ushort refreshDelay;
        public List<PinConfig> pinConfig;
    }

    public class PinConfig
    {
        public byte pin;

        /**
         * known types:
         *  servo
         *  digital
         *  analog
         *  max7seg
         *  analogdisc
         * */
        public string output_type;

        public bool disabled = false;

        public string data_binding;
        // For servo
        public ushort servo_max_range = 90;
        public float servo_step_value = 1f;
        public ushort servo_relative_min = 0;
        public ushort servo_relative_max;
        public bool servo_reverse_rotation = false;

        // Common data
        public bool blink = false;
        public float data_offset = 0f;
        public float data_min_value = 0f;
        public float data_max_value;

        // 7Seg
        public ushort din_pin = 16;
        public ushort cs_pin = 18;
        public ushort clk_pin = 17;
        public ushort digit_length = 4;
        public bool reverse_digit = false;
        public ushort display_offset = 0;
        public ushort max_type = 0; // 0 = 7seg, 1 = matrix
        public string matrix_value_correspondance = "";

        // Anlogdisc
        public string discrete_value;

        public void SetAttributes(Dictionary<string, string> pinConfigs)
        {
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_max_range"), out servo_max_range);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_relative_min"), out servo_relative_min);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_relative_max"), out servo_relative_max);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "din_pin"), out din_pin);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "cs_pin"), out cs_pin);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "clk_pin"), out clk_pin);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "digit_length"), out digit_length);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "display_offset"), out display_offset);
            bool.TryParse(DictUtils.GetString(pinConfigs, "reverse_digit"), out reverse_digit);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "max_type"), out max_type);
            if(max_type == 1)
            {
                // Pour affichage matrix: digit 2 max (clé du tableau MatrixNumber dans Arduino)
                digit_length = 2;
                display_offset = 0;
                reverse_digit = false;
           }

            Single.TryParse(DictUtils.GetString(pinConfigs, "servo_step_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out servo_step_value);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_offset"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_offset);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_min_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_min_value);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_max_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_max_value);

            output_type = DictUtils.GetString(pinConfigs, "output_type", "");
            data_binding = DictUtils.GetString(pinConfigs, "data_binding", "");
            discrete_value = DictUtils.GetString(pinConfigs, "discrete_value", "");
            matrix_value_correspondance = DictUtils.GetString(pinConfigs, "matrix_value_correspondance", "");

            bool.TryParse(DictUtils.GetString(pinConfigs, "servo_reverse_rotation"), out servo_reverse_rotation);
            bool.TryParse(DictUtils.GetString(pinConfigs, "disabled"), out disabled);
            bool.TryParse(DictUtils.GetString(pinConfigs, "blink"), out blink);

        }
    }

    public class GMTConfig
    {
        protected IniData Data;
        public Dictionary<byte, BoardConfig> Boards = new Dictionary<byte, BoardConfig>();
        
        public GMTConfig()
        {
            FileIniDataParser deviceConfig = new FileIniDataParser();
            try
            {
                Data = deviceConfig.ReadFile("GMTHub.ini");
                GetBoards();
            }
            catch (Exception ex)
            {
                ConsoleLog.Error("File GMTHUb error: " + ex.Message);
            }
        }

        public bool boardHasConfig(byte boardNumber)
        {
            BoardConfig res;
            return Boards.TryGetValue(boardNumber, out res) && res.pinConfig.Count > 0;
        }

        public BoardConfig GetBoardConfig(byte boardNumber)
        {
            BoardConfig res;
            if(Boards.TryGetValue(boardNumber, out res) && res.pinConfig.Count > 0)
            {
                return res;
            }
            return null;
        }

        protected void GetBoards()
        {
            // List<int> boardsNumber = new List<int>();
            foreach (var item in Data.Sections)
            {
                item.ClearComments();
                // Section BOARD_N
                if (Regex.Match(item.SectionName.Trim(), "^BOARD_[0-9]{1,}$").Success)
                {
                    try
                    {
                        byte boardNumber = byte.Parse(item.SectionName.Replace("BOARD_", "").Trim());
                        // boardsNumber.Add(boardNumber);
                        string name = (item.Keys["board_name"] ?? item.SectionName).Split(';')[0].Trim();
                        ushort refreshDelay;
                        if(! ushort.TryParse(item.Keys["fps"], out refreshDelay))
                        {
                            refreshDelay = 25; // Correspond à 40fps
                        } else
                        {
                            refreshDelay = (ushort)(1000 / refreshDelay);
                        }
                        Boards.Add(boardNumber, new BoardConfig()
                        {
                            name = name,
                            refreshDelay = refreshDelay,
                            pinConfig = new List<PinConfig>(),
                        });
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Config error - " + item.SectionName + ": " + ex.Message);
                    }
                }
                // Section BOARD_N.PIN_X
                else if(Regex.Match(item.SectionName.Trim(), @"^BOARD_[0-9]{1,}\.PIN_[0-9]{1,}$").Success)
                {
                    try
                    {
                        string[] boardPin = item.SectionName.Trim().Split('.');
                        byte boardNumber = byte.Parse(boardPin[0].Replace("BOARD_", "").Trim());
                        byte pinNumber = byte.Parse(boardPin[1].Replace("PIN_", "").Trim());
                        Dictionary<string, string> pinConfigs = new Dictionary<string, string>();
                        foreach (KeyData key in item.Keys)
                        {
                            // Split ; pour enlever les comments
                            pinConfigs.Add(key.KeyName.Trim(), key.Value.Split(';')[0].Trim());
                        }

                        PinConfig pinConfig = new PinConfig()
                        {
                            pin = pinNumber,
                        };
                        pinConfig.SetAttributes(pinConfigs);
                        Boards[boardNumber].pinConfig.Add(pinConfig);
                    }
                    catch (Exception ex)
                    {
                        ConsoleLog.Error("Config error - " + item.SectionName + ": " + ex.Message);
                    }
                }
            }
            
        }
    }
}
