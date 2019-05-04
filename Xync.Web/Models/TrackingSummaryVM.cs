using System.Collections.Generic;
using Xync.Abstracts;
using Xync.Utils;

namespace Xync.Web.Models
{
    public class TrackingSummaryVM
    {
        public int TotalMappings { get; set; }
        public IList<ITable> Mappings { get; set; }
        public IList<Error> Errors { get; set; }
    }
}