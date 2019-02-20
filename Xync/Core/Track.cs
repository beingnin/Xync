using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts.Core;

namespace Xync.Core
{
    public class Track: ITrack
    {
       public string CDCSchema { get; set; }
       public string CDCTable { get; set; }
       public string TableSchema { get; set; }
       public string TableName { get; set; }
    }
}
