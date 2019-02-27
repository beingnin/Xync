
using System;
using System.Collections.Generic;

namespace Xync.Console.MDO.EGATE
{
    public class Comment
    {
        public long Id { get; set; }
        public Case Case { get; set; }
        public string Value { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
        public Comment Parent { get; set; }
        public List<Comment> Replies { get; set; }
    }
}
