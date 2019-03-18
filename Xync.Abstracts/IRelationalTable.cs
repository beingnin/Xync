using System;

namespace Xync.Abstracts
{
    public interface IRelationalTable<TDocumentModel>:ITable
    {
        Type DocumentModelType { get;  }
        TDocumentModel DocumentModel { get; }
        
        TDocumentModel GetFromMongo(object identifier);
        void DeleteFromMongo(object identifier);
        TDocumentModel CreateModel();
        IRelationalAttribute GetKey();
    }
    
}
