using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public abstract class Synchronizer : ISynchronizer
    {
        public abstract string ConnectionString { get; }
        public static IList<ITable> Monitors { get; set; } = new List<ITable>();
        public abstract void ListenAll(Action<object, EventArgs> onStop = null, Action<object, EventArgs> onResume = null);
        public abstract int Listen(string tblName);
        public virtual IList<ITable> this[string tblName,string schema="dbo"]
        {
            get
            {
                return Monitors.Where(x => x.Name == tblName && x.Schema==schema).ToList();
            }
        }
        public abstract Task<bool> Migrate(string table, string schema);
        public abstract Task<object> GetCounts(string table, string schema, string collection);

    }
}
