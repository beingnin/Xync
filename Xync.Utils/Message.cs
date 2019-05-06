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
        public static event WroteEventHandler WroteError;
        public static event WroteEventHandler WroteInfo;
        public static event WroteEventHandler WroteSuccess;
        public static event WroteEventHandler WroteLoading;
        public async static Task Error(Exception ex, string title = "")
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(ex);
            Console.ResetColor();
            await Logger.Error(ex, title);
            AfterWroteError(MessageType.Error, ex, title);
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
            AfterWroteInfo(MessageType.Info, message, title);
        }
        public async static Task Success(string message, string title)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine(title);
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine(message);
            Console.ResetColor();
            await Logger.Success(message, title);
            AfterWroteSuccess(MessageType.Error, message, title);
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
            AfterWroteLoading(MessageType.Error, message, title);
        }
        private static void AfterWroteError(MessageType t, Exception ex, string title)
        {
            if (WroteError != null)
            {
                WroteError(null, new MessageWroteEventArgs(t, ex, ex.Message, title));
            }
        }
        private static void AfterWroteInfo(MessageType t, string msg, string title)
        {
            if (Message.WroteInfo != null)
            {
                WroteInfo(null, new MessageWroteEventArgs(t, null, msg, title));
            }
        }
        private static void AfterWroteSuccess(MessageType t, string msg, string title)
        {
            if (WroteSuccess != null)
            {
                WroteSuccess(null, new MessageWroteEventArgs(t, null, msg, title));
            }
        }
        private static void AfterWroteLoading(MessageType t, string msg, string title)
        {
            if (WroteLoading != null)
            {
                WroteLoading(null, new MessageWroteEventArgs(t, null, msg, title));
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
