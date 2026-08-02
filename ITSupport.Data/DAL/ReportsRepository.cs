using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class ReportsRepository
    {
        public List<StatusCount> GetCaseCountsByStatus()
        {
            var list = new List<StatusCount>();
            const string sql = @"
                SELECT cs.StatusName, COUNT(*) AS Cnt
                FROM   Cases c
                JOIN   CaseStatuses cs ON cs.StatusId = c.StatusId
                GROUP BY cs.StatusName
                ORDER BY Cnt DESC";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StatusCount
                        {
                            StatusName = (string)reader["StatusName"],
                            Count = (int)reader["Cnt"]
                        });
                    }
                }
            }
            return list;
        }

        public List<PriorityCount> GetCaseCountsByPriority()
        {
            var list = new List<PriorityCount>();
            const string sql = @"
                SELECT p.PriorityName, COUNT(*) AS Cnt
                FROM   Cases c
                JOIN   Priorities p ON p.PriorityId = c.PriorityId
                GROUP BY p.PriorityName
                ORDER BY Cnt DESC";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PriorityCount
                        {
                            PriorityName = (string)reader["PriorityName"],
                            Count = (int)reader["Cnt"]
                        });
                    }
                }
            }
            return list;
        }

        public List<DailyResolvedCount> GetResolvedCountsByDay(int days)
        {
            var list = new List<DailyResolvedCount>();
            const string sql = @"
                SELECT CAST(c.ClosedAt AS DATE) AS ClosedDate, COUNT(*) AS Cnt
                FROM   Cases c
                WHERE  c.ClosedAt IS NOT NULL
                AND    c.ClosedAt >= DATEADD(DAY, -@Days, CAST(GETUTCDATE() AS DATE))
                GROUP BY CAST(c.ClosedAt AS DATE)
                ORDER BY ClosedDate";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Days", days);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DailyResolvedCount
                        {
                            Date = (DateTime)reader["ClosedDate"],
                            Count = (int)reader["Cnt"]
                        });
                    }
                }
            }
            return list;
        }

        public List<EngineerWorkload> GetEngineerWorkload()
        {
            var list = new List<EngineerWorkload>();
            const string sql = @"
                SELECT se.Name AS EngineerName,
                       SUM(CASE WHEN cs.IsOpen = 1 THEN 1 ELSE 0 END) AS OpenCases,
                       COUNT(*) AS TotalCases
                FROM   Cases c
                JOIN   SupportEngineers se ON se.EngineerId = c.OwnerId
                JOIN   CaseStatuses     cs ON cs.StatusId   = c.StatusId
                GROUP BY se.Name
                ORDER BY OpenCases DESC";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new EngineerWorkload
                        {
                            EngineerName = (string)reader["EngineerName"],
                            OpenCases = (int)reader["OpenCases"],
                            TotalCases = (int)reader["TotalCases"]
                        });
                    }
                }
            }
            return list;
        }

        public List<SlaCompliance> GetSlaCompliance()
        {
            var list = new List<SlaCompliance>();
            const string sql = @"
                SELECT p.PriorityName, p.SlaHours,
                       COUNT(*) AS TotalClosed,
                       SUM(CASE WHEN DATEDIFF(MINUTE, c.CreatedAt, c.ClosedAt) <= p.SlaHours * 60 THEN 1 ELSE 0 END) AS WithinSla
                FROM   Cases c
                JOIN   Priorities p ON p.PriorityId = c.PriorityId
                WHERE  c.ClosedAt IS NOT NULL
                GROUP BY p.PriorityName, p.SlaHours
                ORDER BY p.SlaHours";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int totalClosed = (int)reader["TotalClosed"];
                        int withinSla = (int)reader["WithinSla"];

                        list.Add(new SlaCompliance
                        {
                            PriorityName = (string)reader["PriorityName"],
                            SlaHours = (int)reader["SlaHours"],
                            TotalClosed = totalClosed,
                            WithinSla = withinSla,
                            CompliancePercent = totalClosed == 0 ? 0 : Math.Round(withinSla * 100.0 / totalClosed, 1)
                        });
                    }
                }
            }
            return list;
        }
    }
}
