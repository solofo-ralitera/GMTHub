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
using System.IO;

namespace GMTHub.Utils
{
    public class BoardConfig
    {
        public string name;
        public byte boardNumber;
        public int refreshDelay;
        public List<PinConfig> pinConfig;
    }

    public class PinConfig
    {
        public byte pin;

        /// <summary>
        /// known types:
        ///     servo
        ///     digital
        ///     analog
        ///     max7seg
        ///     analogdisc
        ///     frequency (pulse by frequency. E.g. tacho, speedo)
        /// </summary>
        public string output_type;

        public bool disabled = false;

        public string data_binding;
        // For servo
        public ushort servo_max_range = 90;
        public float servo_step_value = 1f;
        public ushort servo_relative_min = 0;
        public ushort servo_relative_max;
        public bool servo_reverse_rotation = false;

        // From factor devices (tacho, speedo)
        /// <summary>
        /// Min  frequency (hertz) accepted by the device
        /// </summary>
        public ushort device_min_range = 0;
        /// <summary>
        /// Max  frequency (hertz) accepted by the device
        /// </summary>
        public ushort device_max_range = 360;
        /// <summary>
        /// unit/hertz (ex: rpm/hertz, km/hertz)
        /// </summary>
        public float device_step_value = 1;

        // Common data
        public ushort blink = 0;
        public string blink_if = "";
        public float data_offset = 0f;
        public float data_min_value = 0f;
        public float data_max_value;
        public long cache;

        // 7Seg, mode SPI hardware, pas besoin de din et clk
        // public ushort din_pin = 11;
        public ushort cs_pin = 12;
        // public ushort clk_pin = 13;
        public ushort digit_length = 4;
        public bool reverse_digit = false;
        public ushort display_offset = 0;
        /// <summary>
        /// 7seg
        /// matrix
        /// extension
        /// </summary>
        public string max_type = "7seg";
        public string matrix_value_correspondance = "";

        // Anlogdisc
        public string discrete_value;

        public List<PinConfig> pinExtensions = new List<PinConfig>();

        public void SetAttributes(Dictionary<string, string> pinConfigs)
        {
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_max_range"), out servo_max_range);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_relative_min"), out servo_relative_min);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "servo_relative_max"), out servo_relative_max);
            // ushort.TryParse(DictUtils.GetString(pinConfigs, "din_pin"), out din_pin);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "cs_pin"), out cs_pin);
            // ushort.TryParse(DictUtils.GetString(pinConfigs, "clk_pin"), out clk_pin);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "digit_length"), out digit_length);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "display_offset"), out display_offset);
            bool.TryParse(DictUtils.GetString(pinConfigs, "reverse_digit"), out reverse_digit);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "blink"), out blink);
            long.TryParse(DictUtils.GetString(pinConfigs, "cache"), out cache);

            Single.TryParse(DictUtils.GetString(pinConfigs, "servo_step_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out servo_step_value);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_offset"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_offset);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_min_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_min_value);
            Single.TryParse(DictUtils.GetString(pinConfigs, "data_max_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out data_max_value);

            Single.TryParse(DictUtils.GetString(pinConfigs, "device_step_value"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out device_step_value);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "device_min_range"), out device_min_range);
            ushort.TryParse(DictUtils.GetString(pinConfigs, "device_max_range"), out device_max_range);


            blink_if = DictUtils.GetString(pinConfigs, "blink_if", "");
            output_type = DictUtils.GetString(pinConfigs, "output_type", "");
            data_binding = DictUtils.GetString(pinConfigs, "data_binding", "");
            discrete_value = DictUtils.GetString(pinConfigs, "discrete_value", "");
            matrix_value_correspondance = DictUtils.GetString(pinConfigs, "matrix_value_correspondance", "");
            max_type = DictUtils.GetString(pinConfigs, "max_type", "");
            if (max_type == "matrix")
            {
                // Pour affichage matrix: digit 2 max (clé du tableau MatrixNumber dans Arduino)
                digit_length = 2;
                display_offset = 0;
                reverse_digit = false;
            }

            bool.TryParse(DictUtils.GetString(pinConfigs, "servo_reverse_rotation"), out servo_reverse_rotation);
            bool.TryParse(DictUtils.GetString(pinConfigs, "disabled"), out disabled);

        }
    }

    public class GMTConfig
    {
        protected IniData Data;
        public Dictionary<byte, BoardConfig> Boards = new Dictionary<byte, BoardConfig>();
        
        public GMTConfig(string gameIni)
        {
            FileIniDataParser deviceConfig = new FileIniDataParser();
            try
            {
                FileInfo confFile = new FileInfo($"./Config/{gameIni}.ini");
                Data = deviceConfig.ReadFile(confFile.FullName);
                GetBoards();

                FileSystemWatcher watcher = new FileSystemWatcher(confFile.DirectoryName);
                watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;
                watcher.Filter = "*.ini";
                watcher.IncludeSubdirectories = false;
                watcher.EnableRaisingEvents = true;
                watcher.Changed += (object sender, FileSystemEventArgs e) =>
                {
                    Data = deviceConfig.ReadFile(confFile.FullName);
                    GetBoards();
                };
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
                        int refreshDelay;
                        if (! int.TryParse(item.Keys["fps"], out refreshDelay))
                        {
                            refreshDelay = 25; // Correspond à 40fps
                        } else
                        {
                            refreshDelay = (int)(1000 / refreshDelay);
                        }
                        if(Boards.ContainsKey(boardNumber))
                        {
                            Boards.Remove(boardNumber);
                        }
                        Boards.Add(boardNumber, new BoardConfig()
                        {
                            name = name,
                            refreshDelay = refreshDelay,
                            boardNumber = boardNumber,
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
                // Section pin extension BOARD_N.PIN_X.Y
                else if (Regex.Match(item.SectionName.Trim(), @"^BOARD_[0-9]{1,}\.PIN_[0-9]{1,}\.[0-9]{1,}$").Success)
                {
                    string[] boardPin = item.SectionName.Trim().Split('.');
                    byte boardNumber = byte.Parse(boardPin[0].Replace("BOARD_", "").Trim());
                    byte pinNumber = byte.Parse(boardPin[1].Replace("PIN_", "").Trim());
                    byte extensionNumber = byte.Parse(boardPin[2].Trim());
                    PinConfig parentPinConfig = GetBoardConfig(boardNumber).pinConfig.Find(pinConfig => pinConfig.pin == pinNumber);
                    PinConfig pinExtentionConfig = new PinConfig()
                    {
                        pin = extensionNumber,
                        output_type = "digital", // Uniquement digital supporté par max72
                    };
                    // TODO refactor avec le if précédent
                    Dictionary<string, string> pinConfigs = new Dictionary<string, string>();
                    foreach (KeyData key in item.Keys)
                    {
                        // Split ; pour enlever les comments
                        pinConfigs.Add(key.KeyName.Trim(), key.Value.Split(';')[0].Trim());
                    }
                    pinExtentionConfig.SetAttributes(pinConfigs);

                    ObjectUtils.CopyValues(parentPinConfig, pinExtentionConfig);
                    parentPinConfig.pinExtensions.Add(pinExtentionConfig);
                }
                // Section LCD page BOARD_N.PIN_X.PAGE_Y
                else if (Regex.Match(item.SectionName.Trim(), @"^BOARD_[0-9]{1,}\.PIN_[0-9]{1,}\.PAGE_[0-9]{1,}$").Success)
                {
                    string[] boardPin = item.SectionName.Trim().Split('.');
                    byte boardNumber = byte.Parse(boardPin[0].Replace("BOARD_", "").Trim());
                    byte pinNumber = byte.Parse(boardPin[1].Replace("PIN_", "").Trim());
                    byte pageNumber = byte.Parse(boardPin[2].Replace("PAGE_", "").Trim());
                    PinConfig parentPinConfig = GetBoardConfig(boardNumber).pinConfig.Find(pinConfig => pinConfig.pin == pinNumber);
                    PinConfig pinExtentionConfig = new PinConfig()
                    {
                        pin = pageNumber,
                        output_type = "lcd",
                    };
                    // TODO refactor avec le if précédent
                    Dictionary<string, string> pinConfigs = new Dictionary<string, string>();
                    foreach (KeyData key in item.Keys)
                    {
                        // Split ; pour enlever les comments
                        pinConfigs.Add(key.KeyName.Trim(), key.Value.Split(';')[0].Trim());
                    }
                    pinExtentionConfig.SetAttributes(pinConfigs);

                    ObjectUtils.CopyValues(parentPinConfig, pinExtentionConfig);
                    parentPinConfig.pinExtensions.Add(pinExtentionConfig);
                }
            }
            
        }
    }
}
