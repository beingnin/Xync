
using System;

namespace Xync.MDO.EGATE
{
    public class ToDo
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public Case Case { get; set; }
        public Priority Priority { get; set; }
        public bool Completed { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
    }
}
