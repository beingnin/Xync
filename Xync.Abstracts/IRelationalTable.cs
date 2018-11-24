using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface IRelationalTable<TDocumentModel>
    {
        TDocumentModel DocumentModel { get;  }
        Type DocumentModelType { get;  }
        long ObjectId { get; set; }
        string Name { get; set; }
        string Schema { get; set; }
        string DB { get; set; }
        List<IRelationalAttribute> Attributes { get; set; }
        void Listen();
        event RowChangedEventHandler OnRowChange;
        TDocumentModel CreateModel(IRelationalTable<TDocumentModel> tbl);
    }
    public delegate void RowChangedEventHandler(object sender, EventArgs e);
}
