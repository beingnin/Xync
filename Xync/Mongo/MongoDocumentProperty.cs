using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;

namespace Xync.Mongo
{
    public class MongoDocumentProperty: IDocumentProperty
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Type DbType { get; set; }
    }
}
