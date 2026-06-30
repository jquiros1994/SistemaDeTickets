namespace ITSupport.Models
{
    public class SupportEngineer
    {
        public int EngineerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int LevelId { get; set; }
        public int ScheduleId { get; set; }
        public bool IsActive { get; set; }

        public SupportEngineer() { }

        public SupportEngineer(int engineerId, string name, string email, string phoneNumber,
                               int levelId, int scheduleId, bool isActive)
        {
            EngineerId = engineerId;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            LevelId = levelId;
            ScheduleId = scheduleId;
            IsActive = isActive;
        }
    }
}
