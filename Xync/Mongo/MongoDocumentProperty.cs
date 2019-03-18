using System;
using Xync.Abstracts;

namespace Xync.Mongo
{
    public class MongoDocumentProperty: IDocumentProperty
    {
        public string Name { get; set; }
        public Type DbType { get; set; }
    }
}
