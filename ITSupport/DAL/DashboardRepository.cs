using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class DashboardRepository
    {
        public int GetOpenCasesCount()
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM   Cases c
                JOIN   CaseStatuses cs ON cs.StatusId = c.StatusId
                WHERE  cs.IsOpen = 1";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd  = new SqlCommand(sql, conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetCriticalCasesCount()
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM   Cases c
                JOIN   CaseStatuses cs ON cs.StatusId  = c.StatusId
                JOIN   Priorities   p  ON p.PriorityId = c.PriorityId
                WHERE  cs.IsOpen = 1
                AND    p.PriorityName = 'Critical'";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd  = new SqlCommand(sql, conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetResolvedTodayCount()
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM   Cases c
                JOIN   CaseStatuses cs ON cs.StatusId = c.StatusId
                WHERE  cs.StatusName IN ('Resolved', 'Closed')
                AND    CAST(c.UpdatedAt AS DATE) = CAST(GETUTCDATE() AS DATE)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd  = new SqlCommand(sql, conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public int GetActiveEngineersCount()
        {
            const string sql = "SELECT COUNT(*) FROM SupportEngineers WHERE IsActive = 1";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd  = new SqlCommand(sql, conn))
            {
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<CaseViewModel> GetRecentCases(int top = 10)
        {
            var list = new List<CaseViewModel>();
            var sql = $@"
                SELECT TOP (@Top)
                       c.CaseId, c.CaseNumber, c.Title,
                       cs.StatusName, cs.IsOpen,
                       p.PriorityName, p.SlaHours,
                       ct.ContactName, ct.ContactEmail,
                       cust.FirstName + ' ' + cust.LastName AS CustomerName,
                       se.Name  AS OwnerName,
                       pr.ProgramName,
                       c.Country, c.PreferredContact,
                       c.CreatedAt, c.UpdatedAt, c.ClosedAt
                FROM   Cases c
                JOIN   CaseStatuses    cs   ON cs.StatusId    = c.StatusId
                JOIN   Priorities      p    ON p.PriorityId   = c.PriorityId
                JOIN   Contacts        ct   ON ct.ContactId   = c.ContactId
                JOIN   Customers       cust ON cust.CustomerId = ct.CustomerId
                JOIN   Programs        pr   ON pr.ProgramId   = c.ProgramId
                LEFT JOIN SupportEngineers se ON se.EngineerId = c.OwnerId
                ORDER BY c.CreatedAt DESC";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd  = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Top", top);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CaseViewModel
                        {
                            CaseId          = (int)reader["CaseId"],
                            CaseNumber      = (string)reader["CaseNumber"],
                            Title           = (string)reader["Title"],
                            StatusName      = (string)reader["StatusName"],
                            IsOpen          = (bool)reader["IsOpen"],
                            PriorityName    = (string)reader["PriorityName"],
                            SlaHours        = (int)reader["SlaHours"],
                            ContactName     = (string)reader["ContactName"],
                            ContactEmail    = reader["ContactEmail"] as string,
                            CustomerName    = (string)reader["CustomerName"],
                            OwnerName       = reader["OwnerName"] as string,
                            ProgramName     = (string)reader["ProgramName"],
                            Country         = (string)reader["Country"],
                            PreferredContact= reader["PreferredContact"] as string,
                            CreatedAt       = (System.DateTime)reader["CreatedAt"],
                            UpdatedAt       = (System.DateTime)reader["UpdatedAt"],
                            ClosedAt        = reader["ClosedAt"] as System.DateTime?
                        });
                    }
                }
            }
            return list;
        }
    }
}
