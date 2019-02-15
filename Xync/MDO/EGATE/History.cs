
using System;
using System.Collections.Generic;

namespace Xync.MDO.EGATE
{
    public class History
    {
        public int HistoryId { get; set; }
        public HistoryTemplate Template { get; set; }
        public string Text1En { get; set; }
        public string Text2En { get; set; }
        public string Text3En { get; set; }
        public string Text4En { get; set; }
        public string Text5En { get; set; }
        public string Text1Ar { get; set; }
        public string Text2Ar { get; set; }
        public string Text3Ar { get; set; }
        public string Text4Ar { get; set; }
        public string Text5Ar { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string Data4 { get; set; }
        public string Data5 { get; set; }
        public Case Case { get; set; }
        public string URL { get; set; }
        public DateTime TimeStampUTC { get; set; }        
    }
}
