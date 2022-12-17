using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GMTHubLib.Utils
{
    public interface IConsoleTextBox
    {
        void AppendText(string text, string color);
    }

    public static class ConsoleLog
    {
        public static IConsoleTextBox textBox = null;
        
        public static void Debug(string message)
        {
#if DEBUG
            if(textBox != null)
            {
                textBox.AppendText(message, "YellowGreen");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                Console.ResetColor();
            }
#else
            
#endif
        }


        public static void Info(string message)
        {
            try
            {
                if (textBox != null)
                {
                    textBox.AppendText(message, "Blue");
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Gray(string message)
        {
            try
            {
                if (textBox != null)
                {
                    textBox.AppendText(message, "DarkGray");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Success(string message)
        {
            try
            {
                if (textBox != null)
                {
                    textBox.AppendText(message, "DarkGreen");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Error(string message)
        {
            try
            {
                if (textBox != null)
                {
                    textBox.AppendText(message, "DarkRed");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
