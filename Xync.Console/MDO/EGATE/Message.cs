
using System;
using System.Collections.Generic;

namespace Xync.Console.MDO.EGATE
{
    public class Message
    {
        public long Id { get; set; }
        public Case Case { get; set; }
        public string Value{ get; set; }
        public Message Parent { get; set; }
        public List<Message> Replies { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
    }
}
