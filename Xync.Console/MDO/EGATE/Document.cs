
using System;

namespace Xync.Console.MDO.EGATE
{
    public class Document
    {
        public long Id { get; set; }
        public Case Case { get; set; }
        public bool UserDefined { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string Path{ get; set; }
        public string MIME { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
        public bool IsOutput { get; set; }
        public DocumentType DocumentType { get; set; }
        public Folder Folder { get; set; }

    }
}
