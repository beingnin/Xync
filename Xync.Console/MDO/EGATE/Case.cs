
using System;
using System.Collections.Generic;

//namespace Xync.Console.MDO.EGATE
namespace Xync.Console.MDO.EGATE
{
    public class Case
    {
        public long CaseId { get; set; }
        public string CaseName { get; set; }
        public Guid ProcessId { get; set; }
        public CaseType Type { get; set; }
        public string Number { get; set; }
        public DateTime CreatedUTC { get; set; }
        public DateTime ModifiedUTC { get; set; }
        public DateTime DueDate { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public Case Parent { get; set; }
        public bool Deleted { get; set; }
        public long DocumentId { get; set; }
        public int TotalTODOs { get; set; }
        public int CompletedTODOs { get; set; }
        public int TotalAssignees { get; set; }
        public List<ToDo> Checklist { get; set; }
        public List<Document> Attachments { get; set; }
        public List<Member> Participants { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Message> Messages { get; set; }
        public List<Case> SubTasks { get; set; }
        public List<Reminder> Reminders { get; set; }

    }
}
