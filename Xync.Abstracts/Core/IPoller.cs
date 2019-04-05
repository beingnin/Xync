using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public interface IPoller
    {
        void Listen();
        event EventHandler ChangeDetected;
        event EventHandler Stopped;
        event EventHandler Resumed;
    }
}
