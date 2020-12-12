using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.WPF.POCOs
{
   public class Permission
    {
        public int Id { get; set; }
        public MemberType Role { get; set; }
        public Case Case { get; set; }
        public bool ViewTask { get; set; }
        public bool CreateTask { get; set; }
        public bool EditTask { get; set; }
        public bool DeleteTask { get; set; }
        public bool ViewAttachment { get; set; }
        public bool UploadAttachment { get; set; }
        public bool EditAttachment { get; set; }
        public bool DeleteAttachment { get; set; }

        public bool TaskToDoAdd { get; set; }
        public bool TaskToDoDelete { get; set; }

        public bool EditCase { get; set; }
        public bool AddCaseParticipant { get; set; }
        public bool RemoveCaseParticipant { get; set; }
        public bool DeleteCase { get; set; }
        public bool ChangePermission { get; set; }
    }
}
