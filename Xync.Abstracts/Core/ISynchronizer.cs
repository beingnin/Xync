using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;

namespace Xync.Abstracts.Core
{
    public interface ISynchronizer<TDocumentModel> where TDocumentModel : class
    {
        TDocumentModel DocumentModel { get; set; }
        TDocumentModel CreateDocumentModel(Abstracts.IRelationalTable<TDocumentModel> table);
        TDocumentModel GetFromDocumentDb<Tkey,Tvalue>(Tkey key,Tvalue value);

    }
}
