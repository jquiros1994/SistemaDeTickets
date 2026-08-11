using System;

namespace ITSupport.Models
{
    public class StatusCount
    {
        public string StatusName { get; set; }
        public int Count { get; set; }
    }

    public class PriorityCount
    {
        public string PriorityName { get; set; }
        public int Count { get; set; }
    }

    public class DailyResolvedCount
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class EngineerWorkload
    {
        public string EngineerName { get; set; }
        public int OpenCases { get; set; }
        public int TotalCases { get; set; }
    }

    public class SlaCompliance
    {
        public string PriorityName { get; set; }
        public int SlaHours { get; set; }
        public int TotalClosed { get; set; }
        public int WithinSla { get; set; }
        public double CompliancePercent { get; set; }
    }
}
