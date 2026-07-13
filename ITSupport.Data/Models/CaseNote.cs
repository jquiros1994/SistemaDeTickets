using System;

namespace ITSupport.Models
{
    public class CaseNote
    {
        public int NoteId { get; set; }
        public int CaseId { get; set; }
        public int UserId { get; set; }
        public string NoteText { get; set; }
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }

        public CaseNote() { }

        public CaseNote(int noteId, int caseId, int userId, string noteText,
                        bool isInternal, DateTime createdAt)
        {
            NoteId = noteId;
            CaseId = caseId;
            UserId = userId;
            NoteText = noteText;
            IsInternal = isInternal;
            CreatedAt = createdAt;
        }
    }
}
