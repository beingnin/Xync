using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xync.Console.MDO.EGATE
{
    public enum TemplateCode
    {
        CaseOpened = 0,
        CaseClosed = 1,
        CaseDeleted = 2,
        CaseEdited = 3,
        CaseReopened = 4,
        ChangedDueDate = 5,
        TaskAdded = 6,
        TaskEdited = 7,
        TaskClosed = 8,
        TaskDeleted = 9,
        ParticipantAdded = 10,
        TaskReopened = 11,
        CaseStatusChanged = 12,
        DocumentAdded = 13,
        DocumentDeleted = 14,        
        BulkDocumentsAdded = 15,
        TaskStatusChanged=16,
        ParticipantDeleted = 17
    }
}
