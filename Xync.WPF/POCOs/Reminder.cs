
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.WPF.POCOs
{
    public class Reminder
    {
        public long Id { get; set; }
        public DateTime ReminderDateUTC { get; set; }
        public Case Case { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
    }
}
