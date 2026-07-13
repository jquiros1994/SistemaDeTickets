using System;

namespace ITSupport.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public string TimeZone { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string WorkDays { get; set; }

        public Schedule() { }

        public Schedule(int scheduleId, string scheduleName, string timeZone,
                        TimeSpan startTime, TimeSpan endTime, string workDays)
        {
            ScheduleId = scheduleId;
            ScheduleName = scheduleName;
            TimeZone = timeZone;
            StartTime = startTime;
            EndTime = endTime;
            WorkDays = workDays;
        }
    }
}
