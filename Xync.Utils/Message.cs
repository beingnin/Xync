using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public static class Message
    {
        public delegate void WroteEventHandler(object sender, MessageWroteEventArgs e);
        public static event WroteEventHandler Wrote;
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
            AfterWrote(MessageType.Error, message, title);
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
            AfterWrote(MessageType.Info, message, title);
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
            AfterWrote(MessageType.Success, message, title);

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
            AfterWrote(MessageType.Loading, message, title);
        }
        private static void AfterWrote(MessageType t,string msg,string title)
        {
            if (Wrote != null)
            {
                Wrote(null, new MessageWroteEventArgs(t, msg,title));
            }
        }
        public enum MessageType
        {
            Error,
            Info,
            Success,
            Loading
        }
    }
}
