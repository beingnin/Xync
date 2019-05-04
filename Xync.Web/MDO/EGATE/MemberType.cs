
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Web.MDO.EGATE
{
   public class MemberType
    {
        public int RoleId { get; set; }
        public string RoleNameEn { get; set; }
        public string RoleNameAr { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
        public string StyleClass { get; set; }
        public Permission Permission { get; set; }
    }
}
