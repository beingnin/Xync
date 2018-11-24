using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface IDocumentCollection
    {
        string DB { get; set; }
        ulong Count { get; set; }
        string Name { get; set; }
        IList<IDocumentProperty> Properties { get; set; }
    }
}
