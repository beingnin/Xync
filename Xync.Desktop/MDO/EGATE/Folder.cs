
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Desktop.MDO.EGATE
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string QualifiedName { get; set; }
        public Case Case { get; set; }
        public string Path { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
        public Folder Parent { get; set; }
    }
}
