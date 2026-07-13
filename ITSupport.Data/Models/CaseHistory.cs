using System;

namespace ITSupport.Models
{
    public class CaseHistory
    {
        public int HistoryId { get; set; }
        public int CaseId { get; set; }
        public int ChangedByUserId { get; set; }
        public string FieldChanged { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime ChangedAt { get; set; }

        public CaseHistory() { }

        public CaseHistory(int historyId, int caseId, int changedByUserId,
                           string fieldChanged, string oldValue, string newValue, DateTime changedAt)
        {
            HistoryId = historyId;
            CaseId = caseId;
            ChangedByUserId = changedByUserId;
            FieldChanged = fieldChanged;
            OldValue = oldValue;
            NewValue = newValue;
            ChangedAt = changedAt;
        }
    }
}
