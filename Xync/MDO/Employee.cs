using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.MDO
{
    public class EmployeesMDO
    {
        public long EmpId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime DOB { get; set; }
        public string DOBString { get; set; }
        public short Designation { get; set; }
        public Department Department { get; set; }
    }
    public class Department
    {
        public int DepId { get; set; }
        public string DepName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Branch Branch { get; set; }

    }
    public class Branch
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public Location Location { get; set; }
    }
    public class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
    }
}
