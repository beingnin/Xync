using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Xync.Utils
{
    public class Event
    {

        public ObjectId Id { get; set; }
        public string Message { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string Title { get; set; }
        public string StackTrace { get; set; }
        public string InnerExceptionStackTrace { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public Message.MessageType MessageType { get; set; }
        public DateTime CreatedDateTime { get; set; }
        string _host;
        public string Host
        {
            get
            {
                return _host = Environment.MachineName;
            }
            set
            {
                _host = value;
            }
        }
    }
}
