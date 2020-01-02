using System;

namespace Xync.Abstracts
{
    public interface IRelationalTable<TDocumentModel>:ITable
    {
        Type DocumentModelType { get;  }
        TDocumentModel DocumentModel { get; }
        
        TDocumentModel GetFromMongo(object identifier);
        void DeleteFromMongo(object identifier);
        void ReplaceInMongo(object identifier,TDocumentModel doc);
        void InsertInMongo(object identifier,TDocumentModel doc);
        TDocumentModel CreateModel();
        //IRelationalAttribute GetKey();
    }
    
}
