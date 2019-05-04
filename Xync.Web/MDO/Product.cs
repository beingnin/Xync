using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.MDO
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public Customer Customer{ get; set; }
        public Tax Tax { get; set; }
    }
    public class Tax
    {
        public int Id { get; set; }
        public int Rate { get; set; }
    }
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }
}
