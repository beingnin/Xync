using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;

namespace Xync.Mongo
{
    public class IMongoCollection: IDocumentCollection
    {
        public string DB { get; set; }
        public ulong Count { get; set; }
        public string Name { get; set; }
        public IList<IDocumentProperty> Properties { get; set; }
    }
}
