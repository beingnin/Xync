using System;
using System.Collections.Generic;
using Xync.Abstracts.Core;

namespace Xync.Core
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
