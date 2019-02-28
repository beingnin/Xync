using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public static class Message
    {
        public static void Error(string message, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Info(string message, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Success(string message, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static void Loading(string message, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
