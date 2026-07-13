namespace ITSupport.Models
{
    public class CaseStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public bool IsOpen { get; set; }

        public CaseStatus() { }

        public CaseStatus(int statusId, string statusName, string description, bool isOpen)
        {
            StatusId = statusId;
            StatusName = statusName;
            Description = description;
            IsOpen = isOpen;
        }
    }
}
