
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.WPF.POCOs
{
    public class Member
    {
        public MemberType Role { get; set; }
        public Permission Permissions { get; set; }
        public Boolean IsPromoted { get; set; }
        public Boolean IsMemberOfCase { get; set; }
    }
}
