using System;

namespace ITSupport.Models
{
    public class CaseViewModel
    {
        public int     CaseId           { get; set; }
        public string  CaseNumber       { get; set; }
        public string  Title            { get; set; }
        public string  Description      { get; set; }
        public string Application       { get; set; }
        public string  StatusName       { get; set; }
        public bool    IsOpen           { get; set; }
        public string  PriorityName     { get; set; }
        public int     SlaHours         { get; set; }
        public string  ContactName      { get; set; }
        public string  ContactEmail     { get; set; }
        public string  OwnerName        { get; set; }
        public string  CustomerName     { get; set; }
        public string  ProgramName      { get; set; }
        public string  Country          { get; set; }
        public string  PreferredContact { get; set; }
        public DateTime CreatedAt       { get; set; }
        public DateTime UpdatedAt       { get; set; }
        public DateTime? ClosedAt       { get; set; }
    }
}
