using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts;

namespace Xync.Abstracts.Core
{
    public interface ISynchronizer 
    {
        string ConnectionString { get;  }
        Task<bool> Migrate(string table,string schema);
    }
    

}
