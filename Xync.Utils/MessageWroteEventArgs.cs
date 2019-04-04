using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public class MessageWroteEventArgs:EventArgs
    {
        public MessageWroteEventArgs(Message.MessageType type,string msg,string title)
        {
            this.Message = msg;
            this.Type = type;
            this.Title = title;
        }

        public string Message { get; set; }
        public string Title { get; set; }
        public Message.MessageType Type { get; } 
    }
}
