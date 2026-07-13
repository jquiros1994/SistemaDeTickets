using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ITSupport.Models;

namespace ITSupport.DAL
{
    public class CaseHistoryRepository
    {
        public List<CaseHistory> GetByCaseId(int caseId)
        {
            var list = new List<CaseHistory>();
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT HistoryId, CaseId, ChangedByUserId, FieldChanged, OldValue, NewValue, ChangedAt FROM CaseHistory WHERE CaseId = @CaseId ORDER BY ChangedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", caseId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        list.Add(Map(reader));
            }
            return list;
        }

        public CaseHistory GetById(int historyId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(
                "SELECT HistoryId, CaseId, ChangedByUserId, FieldChanged, OldValue, NewValue, ChangedAt FROM CaseHistory WHERE HistoryId = @HistoryId", conn))
            {
                cmd.Parameters.AddWithValue("@HistoryId", historyId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                    return reader.Read() ? Map(reader) : null;
            }
        }

        public int Insert(CaseHistory history)
        {
            const string sql = @"
                INSERT INTO CaseHistory (CaseId, ChangedByUserId, FieldChanged, OldValue, NewValue)
                OUTPUT INSERTED.HistoryId
                VALUES (@CaseId, @ChangedByUserId, @FieldChanged, @OldValue, @NewValue)";

            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CaseId", history.CaseId);
                cmd.Parameters.AddWithValue("@ChangedByUserId", history.ChangedByUserId);
                cmd.Parameters.AddWithValue("@FieldChanged", history.FieldChanged);
                cmd.Parameters.AddWithValue("@OldValue", (object)history.OldValue ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NewValue", (object)history.NewValue ?? DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // History records are immutable — no Update method
        public bool Delete(int historyId)
        {
            using (var conn = DatabaseHelper.GetConnection())
            using (var cmd = new SqlCommand("DELETE FROM CaseHistory WHERE HistoryId = @HistoryId", conn))
            {
                cmd.Parameters.AddWithValue("@HistoryId", historyId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private CaseHistory Map(SqlDataReader r) => new CaseHistory(
            r.GetInt32(r.GetOrdinal("HistoryId")),
            r.GetInt32(r.GetOrdinal("CaseId")),
            r.GetInt32(r.GetOrdinal("ChangedByUserId")),
            r.GetString(r.GetOrdinal("FieldChanged")),
            r.IsDBNull(r.GetOrdinal("OldValue")) ? null : r.GetString(r.GetOrdinal("OldValue")),
            r.IsDBNull(r.GetOrdinal("NewValue")) ? null : r.GetString(r.GetOrdinal("NewValue")),
            r.GetDateTime(r.GetOrdinal("ChangedAt"))
        );
    }
}
