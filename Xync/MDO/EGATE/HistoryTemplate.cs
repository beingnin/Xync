using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.MDO.EGATE
{
   public class HistoryTemplate
    {
        public int TemplateId { get; set; }
        public TemplateCode Code { get; set; }
        public string TemplateName { get; set; }
        public string Markup { get; set; }
    }
}
