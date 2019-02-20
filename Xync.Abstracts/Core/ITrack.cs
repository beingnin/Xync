using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public interface ITrack
    {
        string CDCSchema { get; set; }
        string CDCTable { get; set; }
        string TableSchema { get; set; }
        string TableName { get; set; }
    }
}
