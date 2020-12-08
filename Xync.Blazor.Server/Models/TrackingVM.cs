using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xync.Abstracts;
using Xync.Utils;

namespace Xync.Blazor.Server.Models
{
    public class TrackingVM
    {
        public int TotalMappings { get; set; }
        public double PollingInterval { get; set; }
        public IList<ITable> Mappings { get; set; }
        public IList<Event> Events { get; set; }
        public string RDBMSServer { get; set; }
        public string MongoServer { get; set; }
        public string RDBMSDatabase { get; set; }
        public string MongoDatabase { get; set; }
    }
}
