using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface IRelationalTable<TDocumentModel>:ITable
    {
        Type DocumentModelType { get;  }
        TDocumentModel DocumentModel { get; }
        
        TDocumentModel GetFromMongo(object identifier);
        TDocumentModel CreateModel();
        IRelationalAttribute GetKey();
    }
    
}
