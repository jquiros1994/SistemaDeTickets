using System;
using System.Collections.Generic;
using System.Linq;
using ITSupport.DAL;
using ITSupport.Models;

namespace ITSupport.Business
{
    public class ReportsBusiness
    {
        private readonly ReportsRepository _reports = new ReportsRepository();

        public List<StatusCount> GetCaseCountsByStatus()
        {
            return _reports.GetCaseCountsByStatus();
        }

        public List<PriorityCount> GetCaseCountsByPriority()
        {
            return _reports.GetCaseCountsByPriority();
        }

        // Fills day gaps with zero so the chart shows a continuous trend line.
        public List<DailyResolvedCount> GetResolvedTrend(int days)
        {
            var raw = _reports.GetResolvedCountsByDay(days).ToDictionary(d => d.Date.Date, d => d.Count);
            var series = new List<DailyResolvedCount>();
            var today = DateTime.UtcNow.Date;

            for (int i = days - 1; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                int count;
                series.Add(new DailyResolvedCount
                {
                    Date = date,
                    Count = raw.TryGetValue(date, out count) ? count : 0
                });
            }

            return series;
        }

        public List<EngineerWorkload> GetEngineerWorkload()
        {
            return _reports.GetEngineerWorkload();
        }

        public List<SlaCompliance> GetSlaCompliance()
        {
            return _reports.GetSlaCompliance();
        }
    }
}
