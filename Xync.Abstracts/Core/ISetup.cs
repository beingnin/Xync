using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Abstracts.Core
{
    public interface ISetup
    {
        string ConnectionString { get; }
        Task<bool> Initialize();
        Task<bool> ReInitialize();
        Task<bool> DisableOnTable(string table, string schema);
        Task<bool> EnableOnTable(string table, string schema);
        Task<bool> ReEnableOnTable(string table, string schema);
        Task<bool> DisableOnDB();

    }
}
