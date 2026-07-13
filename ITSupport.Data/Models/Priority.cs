namespace ITSupport.Models
{
    public class Priority
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
        public int SlaHours { get; set; }
        public string Description { get; set; }

        public Priority() { }

        public Priority(int priorityId, string priorityName, int slaHours, string description)
        {
            PriorityId = priorityId;
            PriorityName = priorityName;
            SlaHours = slaHours;
            Description = description;
        }
    }
}
