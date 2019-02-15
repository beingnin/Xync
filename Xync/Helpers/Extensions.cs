using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Helpers
{
    public static class Extensions
    {
        public static string Embrace(this string entity)
        {
            var strRemoved = entity.Trim(new char[] { '[', ']' });
            return $"[{strRemoved}]";
        }
    }
}
