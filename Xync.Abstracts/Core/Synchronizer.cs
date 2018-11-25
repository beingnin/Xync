using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public abstract class Synchronizer : ISynchronizer
    {
        public abstract string ConnectionString { get; set; }
        public static List<ITable> Monitors { get; set; } = new List<ITable>();
        public abstract int ListenAll(bool forced = false);
        public abstract int Listen(string tblName);
        public virtual ITable this[string tblName] {
            get
            {
                return Monitors.Where(x => x.Name == tblName).FirstOrDefault();
            }
        }

    }
}
