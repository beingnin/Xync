using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xync.Abstracts.Core;

namespace Xync.Abstracts.Events
{
    public class ChangeDetectedEventArgs : EventArgs
    {
        public ChangeDetectedEventArgs(List<ITrack> tables)
        {
            this.Tables = tables;
        }
        public List<ITrack> Tables { get; set; }
    }
}
