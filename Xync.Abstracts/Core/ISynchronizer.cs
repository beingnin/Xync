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
        Task<bool> Migrate(string table,string schema,int Count);
        Task<bool> ForceSync(string table,string schema);
        Task<object> GetCounts(string table, string schema, string collection);
        void ListenAll(Action<object, EventArgs> onSyncing, Action<object, EventArgs> onStop, Action<object, EventArgs> onResume);
    }
    

}
