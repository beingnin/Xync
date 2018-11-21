using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts
{
    public interface IDocumentProperty
    {
        string Name { get; set; }
        string Key { get; set; }
        Type DbType { get; set; }
    }
}
