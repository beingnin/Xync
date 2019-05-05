using System.Collections.Generic;
using Xync.Abstracts;
using Xync.Utils;

namespace Xync.Web.Models
{
    public class TrackingSummaryVM
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