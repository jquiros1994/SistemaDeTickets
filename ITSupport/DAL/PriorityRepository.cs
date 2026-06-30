using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class PriorityRepository
    {
        public List<Priority> GetAll()
        {
            var list = new List<Priority>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT PriorityId, PriorityName, SlaHours, Description FROM Priorities", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public Priority GetById(int priorityId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT PriorityId, PriorityName, SlaHours, Description FROM Priorities WHERE PriorityId = @PriorityId", conn))
            {
                cmd.Parameters.AddWithValue("@PriorityId", priorityId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(Priority priority)
        {
            const string sql = @"
                INSERT INTO Priorities (PriorityName, SlaHours, Description)
                OUTPUT INSERTED.PriorityId
                VALUES (@PriorityName, @SlaHours, @Description)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PriorityName", priority.PriorityName);
                cmd.Parameters.AddWithValue("@SlaHours", priority.SlaHours);
                cmd.Parameters.AddWithValue("@Description", (object)priority.Description ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Priority priority)
        {
            const string sql = @"
                UPDATE Priorities
                SET PriorityName = @PriorityName, SlaHours = @SlaHours, Description = @Description
                WHERE PriorityId = @PriorityId";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PriorityId", priority.PriorityId);
                cmd.Parameters.AddWithValue("@PriorityName", priority.PriorityName);
                cmd.Parameters.AddWithValue("@SlaHours", priority.SlaHours);
                cmd.Parameters.AddWithValue("@Description", (object)priority.Description ?? DBNull.Value);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int priorityId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM Priorities WHERE PriorityId = @PriorityId", conn))
            {
                cmd.Parameters.AddWithValue("@PriorityId", priorityId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private Priority Map(SqlDataReader r) => new Priority(
            r.GetInt32(r.GetOrdinal("PriorityId")),
            r.GetString(r.GetOrdinal("PriorityName")),
            r.GetInt32(r.GetOrdinal("SlaHours")),
            r.IsDBNull(r.GetOrdinal("Description")) ? null : r.GetString(r.GetOrdinal("Description"))
        );
    }
}
