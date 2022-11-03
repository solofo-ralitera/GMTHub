using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

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
        public Dictionary<string, string> config;
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
                        string name = item.Keys["board_name"] ?? item.SectionName;
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
                        Dictionary<string, string> pinConfig = new Dictionary<string, string>();
                        foreach (KeyData key in item.Keys)
                        {
                            pinConfig.Add(key.KeyName.Trim(), key.Value.Trim());
                        }
                        Boards[boardNumber].pinConfig.Add(new PinConfig()
                        {
                            pin = pinNumber,
                            config = pinConfig
                        });
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
