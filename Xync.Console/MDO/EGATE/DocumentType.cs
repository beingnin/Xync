
using System;

namespace Xync.Console.MDO.EGATE
{
    public class DocumentType
    {
        //future class
        public short Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        public string MIME { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
    }
}
