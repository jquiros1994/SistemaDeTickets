using System;

namespace ITSupport.Models
{
    public class Case
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; }   // computed by DB: CASE-000001
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ContactId { get; set; }
        public int ProgramId { get; set; }
        public string Country { get; set; }
        public int? OwnerId { get; set; }
        public int CreatedByUserId { get; set; }
        public string PreferredContact { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public Case() { }

        public Case(int caseId, string caseNumber, int statusId, int priorityId,
                    string title, string description, int contactId, int programId,
                    string country, int? ownerId, int createdByUserId,
                    string preferredContact, DateTime createdAt, DateTime updatedAt, DateTime? closedAt)
        {
            CaseId = caseId;
            CaseNumber = caseNumber;
            StatusId = statusId;
            PriorityId = priorityId;
            Title = title;
            Description = description;
            ContactId = contactId;
            ProgramId = programId;
            Country = country;
            OwnerId = ownerId;
            CreatedByUserId = createdByUserId;
            PreferredContact = preferredContact;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            ClosedAt = closedAt;
        }
    }
}
