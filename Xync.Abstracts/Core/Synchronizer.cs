﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public abstract class Synchronizer : ISynchronizer
    {
        public abstract string ConnectionString { get; }
        public static IList<ITable> Monitors { get; set; } = new List<ITable>();
        public abstract void ListenAll(Action<object, EventArgs> onSyncing, Action<object, EventArgs> onStop = null, Action<object, EventArgs> onResume = null);
        public abstract int Listen(string tblName);
        public virtual IList<ITable> this[string tblName, string schema = "dbo"]
        {
            get
            {
                return Monitors.Where(x => x.Name == tblName && x.Schema == schema).ToList();
            }
        }
        public abstract Task<bool> Migrate(string table, string schema);
        public abstract Task<bool> Migrate(string table, string schema, int Count);
        public abstract Task<bool> ForceSync(string table, string schema);
        public abstract Task<object> GetCounts(string table, string schema, string collection);
        public bool StopTracking(string table, string schema,string collection)
        {
            Monitors.Where(x => x.Name == table 
                               && x.Schema == schema 
                               && !x.DNT
                               && x.Collection == collection).FirstOrDefault().DNT = true;
            return true;
        }
        public bool StartTracking(string table, string schema, string collection)
        {
            Monitors.Where(x => x.Name == table 
                                && x.Schema == schema 
                                && x.Collection == collection 
                                && x.DNT).FirstOrDefault().DNT = false;
            return true;
        }
    }
}
