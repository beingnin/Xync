using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface ITable
    {
        string Name { get; set; }
        bool DNT { get; set; }
        long ObjectId { get; set; }
        bool HasChange { get; set; }
        IRelationalAttribute this[string col]
        {
            get;
        }
        string Schema { get; set; }
        string DB { get; set; }
        List<IRelationalAttribute> Attributes { get; set; }
        void Listen();
        event RowChangedEventHandler OnRowChange;
    }
    public delegate void RowChangedEventHandler(object sender, EventArgs e);
}
