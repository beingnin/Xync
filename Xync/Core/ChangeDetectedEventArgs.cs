using System;
using System.Collections.Generic;
using Xync.Abstracts.Core;

namespace Xync.Core
{
    public class ChangeDetectedEventArgs : EventArgs
    {
        public ChangeDetectedEventArgs(IEnumerable<ITrack> tables)
        {
            this.Tables = tables;
        }
        public IEnumerable<ITrack> Tables { get; set; }
    }
}
