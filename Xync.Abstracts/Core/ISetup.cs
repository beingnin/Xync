using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public interface ISetup
    {
        string ConnectionString { get;  }
        Task<bool> Initialize();
    }
}
