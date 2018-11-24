using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public class Map
    {
        public IDocumentProperty DocumentProperty { get; set; }
        public SyncDirection Direction { get; set; }
        public Func<object,object> ManipulateByValue { get; set; }
        public Func<object,object> ManipulateByRow { get; set; }

    }
    public enum SyncDirection
    {
        RdbmsToDocument=0,DocumentToRdbms=1
    }
}
