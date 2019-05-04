using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Xync.Utils
{
    public class Error
    {
        public ObjectId Id { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public Exception Exception { get; set; }
    }
}
