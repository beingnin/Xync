using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Utils
{
    public enum Change
    {
        Delete=1,
        Insert=2,
        BeforeUpdate=3,
        AfterUpdate=4
    }
}
